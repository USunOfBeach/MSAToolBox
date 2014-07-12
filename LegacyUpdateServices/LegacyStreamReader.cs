using System;
using System.IO;
using LegacyUpdateServices;

namespace LegacyUpdateServices
{
    class LegacyStreamReader : Stream
    {
        long _endPosition;
        Stream innerStream;
        public event Action<long> Reading;
        public LegacyStreamReader(Stream stream)
        {
            this.innerStream = stream;
            _endPosition = stream.Length;
        }
        public LegacyStreamReader(Stream stream, long count)
        {
            this.innerStream = stream;
            _endPosition = stream.Position + count;
            if (_endPosition > stream.Length)
                _endPosition = stream.Length;
        }
        public LegacyStreamReader(Stream stream, long offset, long count)
        {
            stream.Position = offset > stream.Length ? stream.Length : offset;
            this.innerStream = stream;
            _endPosition = offset + count;
            if (_endPosition > stream.Length)
                _endPosition = stream.Length;
        }
        public override int Read(byte[] array, int offset, int count)
        {
            int readCount = 0;
            if (Position + count > this._endPosition)
                readCount = innerStream.Read(array, offset, (int)(this._endPosition - Position));
            else
                readCount = innerStream.Read(array, offset, count);

            if (Reading == null)
                Reading(Position);

            return readCount;
        }
        public override int ReadByte()
        {
            if (Position >= this._endPosition)
                return -1;
            else
                return base.ReadByte();
        }
        public override bool CanRead
        {
            get { return innerStream.CanRead; }
        }
        public override bool CanSeek
        {
            get { return false; }
        }
        public override bool CanWrite
        {
            get { return false; }
        }
        public override void Flush()
        {
            throw new NotImplementedException();
        }
        public override long Length
        {
            get { return innerStream.Position; }
        }
        public override long Position
        {
            get { return innerStream.Position; }
            set { throw new NotImplementedException(); }
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
