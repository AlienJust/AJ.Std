using System.IO;
using System.Text;

namespace AJ.Std.IO
{
	public class AdvancedBinaryWriter : BinaryWriter {
		private bool _isLittleEndian;
		private readonly byte[] _buffer = new byte[8];

		public AdvancedBinaryWriter(Stream output, Encoding encoding, bool isLittleEndian)
			: base(output, encoding) {
			_isLittleEndian = isLittleEndian;
		}

		public AdvancedBinaryWriter(Stream output, bool isLittleEndian)
			: this(output, Encoding.UTF8, isLittleEndian) { }

		public bool IsLittleEndian {
			get => _isLittleEndian;
			set => _isLittleEndian = value;
		}

		/*
        public override unsafe void Write(double value)
        {
            if (isLittleEndian)
            {
                base.Write(value);
                return;
            }

            ulong num = *((ulong*)&value);
             buffer[7] = (byte)num;
             buffer[6] = (byte)(num >> 8);
             buffer[5] = (byte)(num >> 0x10);
             buffer[4] = (byte)(num >> 0x18);
             buffer[3] = (byte)(num >> 0x20);
             buffer[2] = (byte)(num >> 40);
             buffer[1] = (byte)(num >> 0x30);
             buffer[0] = (byte)(num >> 0x38);
             OutStream.Write(buffer, 0, 8);
        }
        */

		public override void Write(short value) {
			if (_isLittleEndian) {
				base.Write(value);
				return;
			}
			_buffer[1] = (byte)value;
			_buffer[0] = (byte)(value >> 8);
			OutStream.Write(_buffer, 0, 2);
		}

		public override void Write(int value) {
			if (_isLittleEndian) {
				base.Write(value);
				return;
			}
			_buffer[3] = (byte)value;
			_buffer[2] = (byte)(value >> 8);
			_buffer[1] = (byte)(value >> 0x10);
			_buffer[0] = (byte)(value >> 0x18);
			OutStream.Write(_buffer, 0, 4);
		}

		public override void Write(long value) {
			if (_isLittleEndian) {
				base.Write(value);
				return;
			}
			_buffer[7] = (byte)value;
			_buffer[6] = (byte)(value >> 8);
			_buffer[5] = (byte)(value >> 0x10);
			_buffer[4] = (byte)(value >> 0x18);
			_buffer[3] = (byte)(value >> 0x20);
			_buffer[2] = (byte)(value >> 40);
			_buffer[1] = (byte)(value >> 0x30);
			_buffer[0] = (byte)(value >> 0x38);
			OutStream.Write(_buffer, 0, 8);
		}

		/*
        public override unsafe void Write(float value)
        {
            if (isLittleEndian)
            {
                base.Write(value);
                return;
            }
            uint num = *((uint*)&value);
             buffer[3] = (byte)num;
             buffer[2] = (byte)(num >> 8);
             buffer[1] = (byte)(num >> 0x10);
             buffer[0] = (byte)(num >> 0x18);
             OutStream.Write(buffer, 0, 4);
        }
        */
		//[CLSCompliant(false)]
		public override void Write(ushort value) {
			if (_isLittleEndian) {
				base.Write(value);
				return;
			}
			_buffer[1] = (byte)value;
			_buffer[0] = (byte)(value >> 8);
			OutStream.Write(_buffer, 0, 2);
		}

		//[CLSCompliant(false)]
		public override void Write(uint value) {
			if (_isLittleEndian) {
				base.Write(value);
				return;
			}
			_buffer[3] = (byte)value;
			_buffer[2] = (byte)(value >> 8);
			_buffer[1] = (byte)(value >> 0x10);
			_buffer[0] = (byte)(value >> 0x18);
			OutStream.Write(_buffer, 0, 4);
		}

		//[CLSCompliant(false)]
		public override void Write(ulong value) {
			if (_isLittleEndian) {
				base.Write(value);
				return;
			}
			_buffer[7] = (byte)value;
			_buffer[6] = (byte)(value >> 8);
			_buffer[5] = (byte)(value >> 0x10);
			_buffer[4] = (byte)(value >> 0x18);
			_buffer[3] = (byte)(value >> 0x20);
			_buffer[2] = (byte)(value >> 40);
			_buffer[1] = (byte)(value >> 0x30);
			_buffer[0] = (byte)(value >> 0x38);
			OutStream.Write(_buffer, 0, 8);
		}
	}
}