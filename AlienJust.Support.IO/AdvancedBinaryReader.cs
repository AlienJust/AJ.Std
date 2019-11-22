using System.IO;
using System.Text;

namespace AlienJust.Support.IO
{
    /// <summary>
    /// Allows to use big endian bytes order when reading from stream, based on System.IO.BinaryReader
    /// </summary>
    public class AdvancedBinaryReader : BinaryReader
    {
        private bool _isLittleEndian;
        private readonly byte[] _buffer = new byte[8];

        public AdvancedBinaryReader(Stream input, Encoding encoding, bool isLittleEndian)
            : base(input, encoding)
        {
            _isLittleEndian = isLittleEndian;
        }

        public AdvancedBinaryReader(Stream input, bool isLittleEndian)
            : this(input, Encoding.UTF8, isLittleEndian) { }

        public bool IsLittleEndian
        {
            get => _isLittleEndian;
            set => _isLittleEndian = value;
        }

        /*
        public override unsafe double ReadDouble()
        {
            if (isLittleEndian)
                return base.ReadDouble();
            FillMyBuffer(8);
            uint num = (uint)(((buffer[3] | (buffer[2] << 8)) | (buffer[1] << 0x10)) | (buffer[0] << 0x18));
            uint num2 = (uint)(((buffer[7] | (buffer[6] << 8)) | (buffer[5] << 0x10)) | (buffer[4] << 0x18));
            ulong num3 = (num2 << 0x20) | num;
            return *(((double*)&num3));
        }
        */

        public override short ReadInt16()
        {
            if (_isLittleEndian)
                return base.ReadInt16();
            FillMyBuffer(2);
            return (short)(_buffer[1] | (_buffer[0] << 8));
        }

        public override int ReadInt32()
        {
            if (_isLittleEndian)
                return base.ReadInt32();
            FillMyBuffer(4);
            return _buffer[3] | (_buffer[2] << 8) | (_buffer[1] << 0x10) | (_buffer[0] << 0x18);
        }

        public override long ReadInt64()
        {
            if (_isLittleEndian)
                return base.ReadInt64();
            FillMyBuffer(8);
            var num = (uint)(_buffer[3] | (_buffer[2] << 8) | (_buffer[1] << 0x10) | (_buffer[0] << 0x18));
            var num2 = (uint)(_buffer[7] | (_buffer[6] << 8) | (_buffer[5] << 0x10) | (_buffer[4] << 0x18));
            return (num2 << 0x20) | num;
        }

        /*
        public override unsafe float ReadSingle()
        {
            if (isLittleEndian)
                return base.ReadSingle();
            FillMyBuffer(4);
            uint num = (uint)(((buffer[3] | (buffer[2] << 8)) | (buffer[1] << 0x10)) | (buffer[0] << 0x18));
            return *(((float*)&num));
        }
        */
        //[CLSCompliant(false)]
        public override ushort ReadUInt16()
        {
            if (_isLittleEndian)
                return base.ReadUInt16();
            FillMyBuffer(2);
            return (ushort)(_buffer[1] | (_buffer[0] << 8));
        }

        //[CLSCompliant(false)]
        public override uint ReadUInt32()
        {
            if (_isLittleEndian)
                return base.ReadUInt32();
            FillMyBuffer(4);
            return (uint)(_buffer[3] | (_buffer[2] << 8) | (_buffer[1] << 0x10) | (_buffer[0] << 0x18));
        }

        //[CLSCompliant(false)]
        public override ulong ReadUInt64()
        {
            if (_isLittleEndian)
                return base.ReadUInt64();
            FillMyBuffer(8);
            var num = (uint)(_buffer[3] | (_buffer[2] << 8) | (_buffer[1] << 0x10) | (_buffer[0] << 0x18));
            var num2 = (uint)(_buffer[7] | (_buffer[6] << 8) | (_buffer[5] << 0x10) | (_buffer[4] << 0x18));
            return ((num2 << 0x20) | num);
        }

        private void FillMyBuffer(int numBytes)
        {
            int offset = 0;
            int num2 = 0;
            if (numBytes == 1)
            {
                num2 = BaseStream.ReadByte();
                if (num2 == -1)
                {
                    throw new EndOfStreamException("Attempted to read past the end of the stream.");
                }
                _buffer[0] = (byte)num2;
            }
            else
            {
                do
                {
                    num2 = BaseStream.Read(_buffer, offset, numBytes - offset);
                    if (num2 == 0)
                    {
                        throw new EndOfStreamException("Attempted to read past the end of the stream.");
                    }
                    offset += num2;
                } while (offset < numBytes);
            }
        }
    }
}
