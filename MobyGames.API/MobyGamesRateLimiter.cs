using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.RateLimiting;

// ReSharper disable ParameterHidesMember

namespace MobyGames.API;

/// <summary>
/// <see cref="RateLimiter"/> implementation that refreshes allowed permits in a window periodically, but ensures at least that window period between refreshes.
/// </summary>
public sealed class MobyGamesRateLimiter : ReplenishingRateLimiter
{
    private int permitCount;
    private int queueCount;
    private long lastReplenishmentTick;
    private long? idleSince;
    private bool disposed;

    private long failedLeasesCount;
    private long successfulLeasesCount;

    private readonly Timer? renewTimer;
    private readonly FixedWindowRateLimiterOptions options;
    private readonly LinkedList<RequestRegistration> queue = new();

    private object Lock => queue;

    private readonly RateLimitLease successfulLease;
    private readonly RateLimitLease failedLease;
    private static readonly double TickFrequency = (double)TimeSpan.TicksPerSecond / Stopwatch.Frequency;

    /// <inheritdoc />
    public override TimeSpan? IdleDuration => idleSince is null
        ? null
        : new TimeSpan((long)((Stopwatch.GetTimestamp() - idleSince) * TickFrequency));

    /// <inheritdoc />
    public override bool IsAutoReplenishing => options.AutoReplenishment;

    /// <inheritdoc />
    public override TimeSpan ReplenishmentPeriod => options.Window;

    /// <summary>
    /// Initializes the <see cref="MobyGamesRateLimiter"/>.
    /// </summary>
    /// <param name="options">Options to specify the behavior of the <see cref="MobyGamesRateLimiter"/>.</param>
    public MobyGamesRateLimiter(FixedWindowRateLimiterOptions options)
    {
        failedLease = new MobyGamesLease(this, false, null);
        successfulLease = new MobyGamesLease(this, true, null);

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (options.PermitLimit <= 0)
        {
            throw new ArgumentException(
                $"{nameof(options.PermitLimit)} must be set to a value greater than 0.",
                nameof(options)
            );
        }

        if (options.QueueLimit < 0)
        {
            throw new ArgumentException(
                $"{nameof(options.QueueLimit)} must be set to a value greater than or equal to 0.",
                nameof(options)
            );
        }

        if (options.Window <= TimeSpan.Zero)
        {
            throw new ArgumentException(
                $"{nameof(options.Window)} must be set to a value greater than TimeSpan.Zero.",
                nameof(options)
            );
        }

        this.options = new FixedWindowRateLimiterOptions
        {
            PermitLimit = options.PermitLimit,
            QueueProcessingOrder = options.QueueProcessingOrder,
            QueueLimit = options.QueueLimit,
            Window = options.Window,
            AutoReplenishment = options.AutoReplenishment
        };

        permitCount = options.PermitLimit;

        idleSince = lastReplenishmentTick = Stopwatch.GetTimestamp();

        if (this.options.AutoReplenishment)
        {
            renewTimer = new Timer(Replenish, this, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }
    }

    /// <inheritdoc/>
    public override RateLimiterStatistics GetStatistics()
    {
        ThrowIfDisposed();
        return new RateLimiterStatistics
        {
            CurrentAvailablePermits = permitCount,
            CurrentQueuedCount = queueCount,
            TotalFailedLeases = Interlocked.Read(ref failedLeasesCount),
            TotalSuccessfulLeases = Interlocked.Read(ref successfulLeasesCount),
        };
    }

    /// <inheritdoc/>
    protected override RateLimitLease AttemptAcquireCore(int permitCount)
    {
        // These amounts of resources can never be acquired
        // Raises a PermitLimitExceeded ArgumentOutOFRangeException
        if (permitCount > options.PermitLimit)
        {
            throw new ArgumentOutOfRangeException(
                nameof(permitCount),
                permitCount,
                $"{this.permitCount} permit(s) exceeds the permit limit of {options.PermitLimit}."
            );
        }

        // Return SuccessfulLease or FailedLease depending to indicate limiter state
        if (permitCount == 0 && !disposed)
        {
            // Check if the requests are permitted in a window
            // Requests will be allowed if the total served request is less than the max allowed requests (permit limit).
            if (this.permitCount > 0)
            {
                Interlocked.Increment(ref successfulLeasesCount);
                return successfulLease;
            }

            Interlocked.Increment(ref failedLeasesCount);
            return CreateFailedWindowLease(permitCount);
        }

        lock (Lock)
        {
            if (TryLeaseUnsynchronized(permitCount, out RateLimitLease? lease))
            {
                return lease;
            }

            Interlocked.Increment(ref failedLeasesCount);
            return CreateFailedWindowLease(permitCount);
        }
    }

    /// <inheritdoc/>
    protected override ValueTask<RateLimitLease> AcquireAsyncCore(
        int permitCount,
        CancellationToken cancellationToken
    )
    {
        // These amounts of resources can never be acquired
        if (permitCount > options.PermitLimit)
        {
            throw new ArgumentOutOfRangeException(
                nameof(permitCount),
                permitCount,
                $"{this.permitCount} permit(s) exceeds the permit limit of {options.PermitLimit}."
            );
        }

        ThrowIfDisposed();

        // Return SuccessfulAcquisition if permitCount is 0 and resources are available
        if (permitCount == 0 && this.permitCount > 0)
        {
            Interlocked.Increment(ref successfulLeasesCount);
            return new ValueTask<RateLimitLease>(successfulLease);
        }

        lock (Lock)
        {
            if (TryLeaseUnsynchronized(permitCount, out var lease))
            {
                return new ValueTask<RateLimitLease>(lease);
            }

            // Avoid integer overflow by using subtraction instead of addition
            Debug.Assert(options.QueueLimit >= queueCount);
            if (options.QueueLimit - queueCount < permitCount)
            {
                if (options.QueueProcessingOrder == QueueProcessingOrder.NewestFirst &&
                    permitCount <= options.QueueLimit)
                {
                    // remove oldest items from queue until there is space for the newest acquisition request
                    do
                    {
                        var oldestRequest = queue.First!.Value;
                        queue.RemoveFirst();
                        queueCount -= oldestRequest.Count;
                        Debug.Assert(queueCount >= 0);
                        if (!oldestRequest.Tcs.TrySetResult(failedLease))
                        {
                            queueCount += oldestRequest.Count;
                        }
                        else
                        {
                            Interlocked.Increment(ref failedLeasesCount);
                        }
                    } while (options.QueueLimit - queueCount < permitCount);
                }
                else
                {
                    Interlocked.Increment(ref failedLeasesCount);
                    // Don't queue if queue limit reached and QueueProcessingOrder is OldestFirst
                    return new ValueTask<RateLimitLease>(CreateFailedWindowLease(permitCount));
                }
            }

            var tcs = new CancelQueueState(permitCount, this, cancellationToken);
            CancellationTokenRegistration ctr = default;
            if (cancellationToken.CanBeCanceled)
            {
                ctr = cancellationToken.Register(static obj => { ((CancelQueueState)obj!).TrySetCanceled(); }, tcs);
            }

            var registration = new RequestRegistration(permitCount, tcs, ctr);
            queue.AddLast(registration);
            queueCount += permitCount;
            Debug.Assert(queueCount <= options.QueueLimit);

            return new ValueTask<RateLimitLease>(registration.Tcs.Task);
        }
    }

    private RateLimitLease CreateFailedWindowLease(int permitCount)
    {
        var replenishAmount = permitCount - this.permitCount + queueCount;
        // can't have 0 replenish window, that would mean it should be a successful lease
        var replenishWindow = Math.Max(replenishAmount / options.PermitLimit, 1);

        return new MobyGamesLease(this, false, TimeSpan.FromTicks(options.Window.Ticks * replenishWindow));
    }

    private bool TryLeaseUnsynchronized(int permitCount, [NotNullWhen(true)] out RateLimitLease? lease)
    {
        ThrowIfDisposed();

        // if permitCount is 0 we want to queue it if there are no available permits
        if (this.permitCount >= permitCount && this.permitCount != 0)
        {
            if (permitCount == 0)
            {
                Interlocked.Increment(ref successfulLeasesCount);
                // Edge case where the check before the lock showed 0 available permit counters but when we got the lock, some permits were now available
                lease = successfulLease;
                return true;
            }

            // a. If there are no items queued we can lease
            // b. If there are items queued but the processing order is newest first, then we can lease the incoming request since it is the newest
            if (queueCount == 0 || (queueCount > 0 && options.QueueProcessingOrder == QueueProcessingOrder.NewestFirst))
            {
                idleSince = null;
                this.permitCount -= permitCount;
                Debug.Assert(this.permitCount >= 0);
                Interlocked.Increment(ref successfulLeasesCount);
                lease = successfulLease;
                return true;
            }
        }

        lease = null;
        return false;
    }

    /// <summary>
    /// Attempts to replenish request counters in the window.
    /// </summary>
    /// <returns>
    /// False if <see cref="FixedWindowRateLimiterOptions.AutoReplenishment"/> is enabled, otherwise true.
    /// Does not reflect if counters were replenished.
    /// </returns>
    public override bool TryReplenish()
    {
        if (options.AutoReplenishment)
        {
            return false;
        }

        Replenish(this);
        return true;
    }

    private static void Replenish(object? state)
    {
        var limiter = (state as MobyGamesRateLimiter)!;
        Debug.Assert(limiter is not null);

        // Use Stopwatch instead of DateTime.UtcNow to avoid issues on systems where the clock can change
        var nowTicks = Stopwatch.GetTimestamp();
        limiter.ReplenishInternal(nowTicks);
    }

    // Used in tests that test behavior with specific time intervals
    private void ReplenishInternal(long nowTicks)
    {
        // Method is re-entrant (from Timer), lock to avoid multiple simultaneous replenishes
        lock (Lock)
        {
            if (disposed)
            {
                return;
            }

            if (((nowTicks - lastReplenishmentTick) * TickFrequency) < options.Window.Ticks &&
                !options.AutoReplenishment)
            {
                return;
            }

            lastReplenishmentTick = nowTicks;

            var availablePermitCounters = permitCount;

            if (availablePermitCounters >= options.PermitLimit)
            {
                // All counters available, nothing to do
                return;
            }

            permitCount = options.PermitLimit;

            // Process queued requests
            while (queue.Count > 0)
            {
                var nextPendingRequest = options.QueueProcessingOrder == QueueProcessingOrder.OldestFirst
                    ? queue.First!.Value
                    : queue.Last!.Value;

                if (permitCount >= nextPendingRequest.Count)
                {
                    // Request can be fulfilled
                    if (options.QueueProcessingOrder == QueueProcessingOrder.OldestFirst)
                    {
                        nextPendingRequest = queue.First!.Value;
                        queue.RemoveFirst();
                    }
                    else
                    {
                        nextPendingRequest = queue.Last!.Value;
                        queue.RemoveLast();
                    }

                    queueCount -= nextPendingRequest.Count;
                    permitCount -= nextPendingRequest.Count;
                    Debug.Assert(permitCount >= 0);

                    if (!nextPendingRequest.Tcs.TrySetResult(successfulLease))
                    {
                        // Queued item was canceled so add count back
                        permitCount += nextPendingRequest.Count;
                        // Updating queue count is handled by the cancellation code
                        queueCount += nextPendingRequest.Count;
                    }
                    else
                    {
                        Interlocked.Increment(ref successfulLeasesCount);
                    }

                    nextPendingRequest.CancellationTokenRegistration.Dispose();
                    Debug.Assert(queueCount >= 0);
                }
                else
                {
                    // Request cannot be fulfilled
                    break;
                }
            }

            if (permitCount == options.PermitLimit)
            {
                Debug.Assert(idleSince is null);
                Debug.Assert(queueCount == 0);
                idleSince = Stopwatch.GetTimestamp();
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        lock (Lock)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            renewTimer?.Dispose();
            while (queue.Count > 0)
            {
                RequestRegistration next;
                if (options.QueueProcessingOrder == QueueProcessingOrder.OldestFirst)
                {
                    next = queue.First!.Value;
                    queue.RemoveFirst();
                }
                else
                {
                    next = queue.Last!.Value;
                    queue.RemoveLast();
                }

                next.CancellationTokenRegistration.Dispose();
                next.Tcs.TrySetResult(failedLease);
            }
        }
    }

    protected override ValueTask DisposeAsyncCore()
    {
        Dispose(true);

        return default;
    }

    private void ThrowIfDisposed()
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(MobyGamesRateLimiter));
        }
    }

    private void RestartTimer()
    {
        renewTimer?.Change(options.Window, Timeout.InfiniteTimeSpan);
    }

    private sealed class MobyGamesLease : RateLimitLease
    {
        private static readonly string[] AllMetadataNames = { MetadataName.RetryAfter.Name };

        private readonly TimeSpan? retryAfter;

        private readonly MobyGamesRateLimiter rateLimiter;

        public MobyGamesLease(MobyGamesRateLimiter rateLimiter, bool isAcquired, TimeSpan? retryAfter)
        {
            IsAcquired = isAcquired;
            this.retryAfter = retryAfter;
            this.rateLimiter = rateLimiter;
        }

        public override bool IsAcquired { get; }

        public override IEnumerable<string> MetadataNames => AllMetadataNames;

        public override bool TryGetMetadata(string metadataName, out object? metadata)
        {
            if (metadataName == MetadataName.RetryAfter.Name && retryAfter.HasValue)
            {
                metadata = retryAfter.Value;
                return true;
            }

            metadata = default;
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                rateLimiter.RestartTimer();
            }
        }
    }

    private readonly struct RequestRegistration
    {
        public RequestRegistration(
            int permitCount,
            TaskCompletionSource<RateLimitLease> tcs,
            CancellationTokenRegistration cancellationTokenRegistration
        )
        {
            Count = permitCount;
            // Use VoidAsyncOperationWithData<T> instead
            Tcs = tcs;
            CancellationTokenRegistration = cancellationTokenRegistration;
        }

        public int Count { get; }

        public TaskCompletionSource<RateLimitLease> Tcs { get; }

        public CancellationTokenRegistration CancellationTokenRegistration { get; }
    }

    private sealed class CancelQueueState : TaskCompletionSource<RateLimitLease>
    {
        private readonly int permitCount;
        private readonly MobyGamesRateLimiter limiter;
        private readonly CancellationToken cancellationToken;

        public CancelQueueState(int permitCount, MobyGamesRateLimiter limiter, CancellationToken cancellationToken)
            : base(TaskCreationOptions.RunContinuationsAsynchronously)
        {
            this.permitCount = permitCount;
            this.limiter = limiter;
            this.cancellationToken = cancellationToken;
        }

        public new bool TrySetCanceled()
        {
            if (TrySetCanceled(cancellationToken))
            {
                lock (limiter.Lock)
                {
                    limiter.queueCount -= permitCount;
                }

                return true;
            }

            return false;
        }
    }
}
