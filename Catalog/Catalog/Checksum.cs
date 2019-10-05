using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Catalog.Wpf;

namespace Catalog
{
    public static class Checksum
    {
        public static byte[] Generate(Stream inputStream, HashAlgorithm hash, IProgress<int> progress = null)
        {
            if (progress == null)
            {
                return hash.ComputeHash(inputStream);
            }

            var progressStream = new ProgressStream(inputStream);

            progressStream.ProgressChanged += (sender, args) => progress.Report(args.ProgressPercentage);

            return hash.ComputeHash(progressStream);
        }

        public static async Task<byte[]> GenerateAsync(Stream inputStream, HashAlgorithm hash, IProgress<int> progress = null)
        {
            return await Task.Run(() => Generate(inputStream, hash, progress));
        }

        public static byte[] GenerateSha256(Stream inputStream, IProgress<int> progress = null)
        {
            return Generate(inputStream, SHA256.Create(), progress);
        }

        public static Task<byte[]> GenerateSha256Async(Stream inputStream, IProgress<int> progress = null)
        {
            return GenerateAsync(inputStream, SHA256.Create(), progress);
        }
    }
}