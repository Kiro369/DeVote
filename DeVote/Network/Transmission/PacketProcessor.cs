using DeVote.Extensions;
using DeVote.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DeVote.Network.Transmission
{
    public abstract unsafe class PacketProcessor
    {
        protected static readonly LogProxy Log = new("PacketProcessor");

        /// <summary> Guts of a packet, the byte array. </summary>
        public Byte[] Buffer;

        /// <summary> Last recorded operator position. </summary>
        private Int32 _operatorPosition;

        protected PacketProcessor()
        {
        }

        protected PacketProcessor(Byte[] receivedPacket)
        {
            Buffer = receivedPacket;
        }

        protected PacketProcessor(Int32 length)
        {
            Buffer = new byte[length];
        }

        #region Operation Methods

        /// <summary>
        ///     Returns the packet array length.
        /// </summary>
        public int ArrayLength
        {
            get { return Buffer.Length; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int PositionAndAdvance(int advance)
        {
            try
            {
                int request = _operatorPosition + advance;
                if (request > ArrayLength)
                    Log.Error("{0} : Cannot advance past the end of an array.", GetType().Name);
                return _operatorPosition;
            }
            finally
            {
                _operatorPosition += advance;
            }
        }

        public PacketProcessor Seek(Int32 position)
        {
            _operatorPosition = position;
            return this;
        }

        public PacketProcessor SeekForward(Int32 position)
        {
            _operatorPosition += position;
            return this;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void WriteHeader(int length)
        {
            fixed (byte* ptr = Buffer)
                *(short*)(ptr) = (short)length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Resize(int length)
        {
            if (length < 4)
                throw new IndexOutOfRangeException("Packet cannot be resized. Length must be 4 or greater.");


            //Changed from Array.Resize to user code after seeing System.Array.Resize source code.
            //Possible performance improvement from agressive inlining?

            var newArray = new byte[length];
            if (Buffer == null || Buffer.Length == 0) Buffer = new byte[length];
            else
                fixed (byte* newptr = newArray)
                fixed (byte* copyptr = Buffer)
                    for (byte* dst = newptr, src = copyptr; (int)newptr < (int)copyptr;) *dst++ = *src++;
        }

        public void Dispose()
        {
            Buffer = null;
        }

        #endregion

        #region Read Methods

        /// <summary>
        ///     Does not update the array position.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public byte this[int offset]
        {
            get { fixed (byte* ptr = Buffer) return ptr[offset]; }
            set { fixed (byte* ptr = Buffer) ptr[offset] = value; }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBoolean()
        {
            fixed (byte* ptr = Buffer)
                return ptr[PositionAndAdvance(sizeof(byte))] == 1;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte()
        {
            // Read the value:
            fixed (byte* ptr = Buffer)
                return ptr[PositionAndAdvance(sizeof(byte))];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte()
        {
            // Read the value:
            fixed (byte* ptr = Buffer)
                return *(sbyte*)(ptr + PositionAndAdvance(sizeof(sbyte)));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUShort()
        {
            // Read the value:
            fixed (byte* ptr = Buffer)
                return *(ushort*)(ptr + PositionAndAdvance(sizeof(ushort)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadShort()
        {
            // Read the value:
            fixed (byte* ptr = Buffer)
                return *(short*)(ptr + PositionAndAdvance(sizeof(short)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt()
        {
            // Read the value:
            fixed (byte* ptr = Buffer)
                return *(uint*)(ptr + PositionAndAdvance(sizeof(uint)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt()
        {
            // Read the value:
            fixed (byte* ptr = Buffer)
                return *(int*)(ptr + PositionAndAdvance(sizeof(int)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadULong()
        {
            // Read the value:
            fixed (byte* ptr = Buffer)
                return *(ulong*)(ptr + PositionAndAdvance(sizeof(ulong)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadLong()
        {
            fixed (byte* ptr = Buffer)
                return *(long*)(ptr + PositionAndAdvance(sizeof(long)));
        }

        public string ReadString()
        {
            return ReadString(ReadByte());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadString(int length)
        {
            fixed (byte* ptr = Buffer)
                return
                    new string((sbyte*)ptr, PositionAndAdvance(length), length, Encoding.GetEncoding(1252)).TrimEnd(
                        '\0');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string[] ReadStrings()
        {
            var result = new string[ReadByte()];
            for (var i = 0; i < result.Length; i++)
                result[i] = ReadString(ReadByte());
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadString(int length, Encoding encoding)
        {
            fixed (byte* ptr = Buffer)
                return new string((sbyte*)ptr, PositionAndAdvance(length), length, encoding).TrimEnd('\0');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ReadArray(int length)
        {
            var array = new byte[length];
            System.Array.Copy(Buffer, PositionAndAdvance(length), array, 0, length);
            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadVoid(void* type, int size)
        {
            fixed (byte* ptr = Buffer)
                NativeMethods.memcpy(type, ptr + PositionAndAdvance(size), size);
        }

        #endregion

        #region Write Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Fill(byte value, int offset, int length)
        {
            fixed (byte* ptr = Buffer)
                NativeMethods.memset(ptr + offset, value, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBoolean(bool value)
        {
            fixed (byte* ptr = Buffer)
                ptr[PositionAndAdvance(sizeof(byte))] = (byte)(value ? 1 : 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(byte value)
        {
            // Write the value:
            fixed (byte* ptr = Buffer)
                ptr[PositionAndAdvance(sizeof(byte))] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSByte(sbyte value)
        {
            // Write the value:
            fixed (byte* ptr = Buffer)
                *(sbyte*)(ptr + PositionAndAdvance(sizeof(sbyte))) = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUShort(ushort value)
        {
            // Write the value:
            fixed (byte* ptr = Buffer)
                *(ushort*)(ptr + PositionAndAdvance(sizeof(ushort))) = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteShort(short value)
        {
            // Write the value:
            fixed (byte* ptr = Buffer)
                *(short*)(ptr + PositionAndAdvance(sizeof(short))) = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt(uint value)
        {
            // Write the value:
            fixed (byte* ptr = Buffer)
                *(uint*)(ptr + PositionAndAdvance(sizeof(uint))) = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt(int value)
        {
            // Write the value:
            fixed (byte* ptr = Buffer)
                *(int*)(ptr + PositionAndAdvance(sizeof(int))) = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteULong(ulong value)
        {
            // Write the value:
            fixed (byte* ptr = Buffer)
                *(ulong*)(ptr + PositionAndAdvance(sizeof(ulong))) = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteLong(long value)
        {
            // Write the value:
            fixed (byte* ptr = Buffer)
                *(long*)(ptr + PositionAndAdvance(sizeof(long))) = value;
        }

        public void WriteStrings(params string[] value)
        {
            WriteByte((byte)value.Length);
            foreach (string str in value)
                if (!string.IsNullOrEmpty(str))
                    WriteStringWithLength(str);
                else
                    WriteByte(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString(string value, int fill = 0)
        {
            int offset = PositionAndAdvance(value.Length);
            Fill(0, offset, fill > 0 ? fill : value.Length);
            fixed (byte* ptr = Buffer)
                NativeMethods.memcpy(ptr + offset, value, value.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString(string value, Encoding encoding, int fill = 0)
        {
            int offset = PositionAndAdvance(value.Length);
            byte[] bytes = encoding.GetBytes(value);
            Fill(0, offset, fill > 0 ? fill : value.Length);
            fixed (byte* ptr = Buffer)
                NativeMethods.memcpy(ptr + offset, bytes, bytes.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteStringWithLength(string value)
        {
            // Write the value:
            fixed (byte* ptr = Buffer)
            {
                int offset = PositionAndAdvance(value.Length + 1);
                ptr[offset] = (byte)value.Length;
                Fill(0, offset + 1, value.Length);
                NativeMethods.memcpy(ptr + offset + 1, value, value.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteArray(byte[] value)
        {
            int pos = PositionAndAdvance(value.Length);
            Fill(0, pos, value.Length);
            fixed (byte* ptr = Buffer)
                NativeMethods.memcpy(ptr + pos, value, value.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteArray(byte[] value, int length, int offset)
        {
            // Write the value:
            Fill(0, offset, length);
            fixed (byte* ptr = Buffer)
                NativeMethods.memcpy(ptr + offset, value, length);
        }

        public void WriteVoid(void* value, int len)
        {
            fixed (byte* ptr = Buffer)
                NativeMethods.memcpy(ptr + PositionAndAdvance(len), value, len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteRandomInt(int minValue, int maxValue, byte offset)
        {
            fixed (byte* ptr = Buffer)
                *(int*)(ptr + offset) = new Random().Next(minValue, maxValue);
        }

        /// <summary> This method writes a signed random integer to the packet structure. </summary>
        /// <param name="seed">The seed being used to generate the numbers</param>
        /// <param name="minValue">The minimum value that can be generated by the random number generator.</param>
        /// <param name="maxValue">The maximum value that can be generated by the random number generator.</param>
        /// <param name="offset">The position where the value will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteRandomInt(int seed, int minValue, int maxValue, byte offset)
        {
            // Write the value:
            fixed (byte* ptr = Buffer)
                *(int*)(ptr + offset) = new Random(seed).Next(minValue, maxValue);
        }

        #endregion

        #region Build Methods

        /// <summary>
        ///     Returns a complete packet as a byte array.
        /// </summary>
        protected virtual byte[] Build()
        {
            var _out = new byte[Buffer.Length];
            fixed (Byte* src = Buffer, dst = _out)
                NativeMethods.memcpy(dst, src, Buffer.Length);
            return _out;
        }

        /// <summary> This method creates a new string that represents the packet structure. </summary>
        /// <returns>This method returns the string representation of the array.</returns>
        public override string ToString()
        {
            return BitConverter.ToString(Buffer).Replace("-", " ");
        }

        /// <summary> Returns the hash code of the packet array. </summary>
        public override int GetHashCode()
        {
            return Buffer.GetHashCode();
        }

        /// <summary> Returns true if the byte array is equal to this packet. </summary>
        /// <param name="obj">The byte array being compared with this packet.</param>
        public override bool Equals(object obj)
        {
            return Buffer.Equals(obj);
        }

        #endregion

        public static implicit operator Boolean(PacketProcessor p)
        {
            return p != null;
        }
    }
}
