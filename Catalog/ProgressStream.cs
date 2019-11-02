using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Wpf
{
    public class ProgressStream : Stream
    {
        private readonly Stream stream;

        public ProgressStream(Stream stream)
        {
            this.stream = stream;
        }

        public event ProgressChangedEventHandler ProgressChanged;

        protected virtual void OnProgressChanged()
        {
            ProgressChanged?.Invoke(
                this,
                new ProgressChangedEventArgs((int) ((float)Position / Length * 100), null)
            );
        }

        public override void Flush()
        {
            stream.Flush();

            OnProgressChanged();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var result = stream.Read(buffer, offset, count);

            OnProgressChanged();

            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }

        public override bool CanRead => stream.CanRead;
        public override bool CanSeek => stream.CanSeek;
        public override bool CanWrite => stream.CanWrite;
        public override long Length => stream.Length;

        public override long Position
        {
            get => stream.Position;
            set => stream.Position = value;
        }
    }
}