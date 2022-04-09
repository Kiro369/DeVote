using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DeVote.Extensions
{
    public static partial class NativeMethods
    {
        private const string Msvcrt = "msvcrt.dll";

        /// <summary> Copies the value of one memory location into another. </summary>
        /// <param name="dst">The destination of the value being copied.</param>
        /// <param name="src">The source of the copy.</param>
        /// <param name="length">The length of data to be copied.</param>
        [DllImport(Msvcrt, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern unsafe void* memcpy(void* dst, string src, int length);

        /// <summary> Copies the value of one memory location into another. </summary>
        /// <param name="dst">The destination of the value being copied.</param>
        /// <param name="src">The source of the copy.</param>
        /// <param name="length">The length of data to be copied.</param>
        [DllImport(Msvcrt, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern unsafe void* memcpy(void* dst, byte[] src, int length);

        /// <summary> Copies the value of one memory location into another. </summary>
        /// <param name="dst">The destination of the value being copied.</param>
        /// <param name="src">The source of the copy.</param>
        /// <param name="length">The length of data to be copied.</param>
        [DllImport(Msvcrt, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern unsafe void* memcpy(byte[] dst, void* src, int length);

        /// <summary>
        ///     Copies the values of num bytes from the location pointed by source directly to the memory block pointed
        ///     by destination.
        ///     The underlying type of the objects pointed by both the source and destination pointers are irrelevant for
        ///     this function; The result is a binary copy of the data.
        ///     The function does not check for any terminating null character in source - it always copies exactly numbytes.
        ///     To avoid overflows, the size of the arrays pointed by both the destination and source parameters, shall be
        ///     at least num bytes, and should not overlap (for overlapping memory blocks, memmove is a safer approach).
        /// </summary>
        [DllImport(Msvcrt, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern unsafe void* memcpy(void* dst, void* src, int length);

        /// <summary>
        ///     Sets the first num bytes of the block of memory pointed by ptr to the specified value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* memset(void* ptr, Byte value, Int32 num)
        {
            for (Int32 i = 0; i < num; i++)
                *(((Byte*)ptr) + i) = value;
            return ptr;
        }

        /// <summary>
        ///     Allocates a block of size bytes of memory, returning a pointer to the beginning of the block.
        ///     The content of the newly allocated block of memory is not initialized, remaining with indeterminate values.
        /// </summary>
        public static unsafe void* malloc(Int32 size)
        {
            void* ptr = Marshal.AllocHGlobal(size).ToPointer();
            return ptr;
        }

        /// <summary>
        ///     A block of memory previously allocated using a call to malloc, calloc or realloc is deallocated, making
        ///     it available again for further allocations.
        ///     The pointer is not set to NULL. It has to be set manually.
        /// </summary>
        public static unsafe void free(void* ptr)
        {
            if (ptr != null)
                Marshal.FreeHGlobal((IntPtr)ptr);
        }

        /// <summary>
        ///     Returns the length of str.
        ///     The length of a C string is determined by the terminating null-character: A C string is as long as
        ///     the amount of characters between the beginning of the string and the terminating null character.
        ///     This should not be confused with the size of the array that holds the string.
        /// </summary>
        public static unsafe Int32 strlen(Byte* ptr)
        {
            for (int i = 0; i < Int32.MaxValue; i++)
                if (ptr[i] == '\0')
                    return i;
            return Int32.MaxValue;
        }

        /// <summary>
        ///     Get a pointer to the Windows-1252 encoded string.
        ///     It will create a null-terminating string...
        /// </summary>
        public static unsafe Byte* ToPointer(this String Str)
        {
            byte[] Buffer = Encoding.GetEncoding(1252).GetBytes(Str + "\0");
            var ptr = (Byte*)malloc(Buffer.Length);

            fixed (Byte* pBuffer = Buffer)
                memcpy(ptr, pBuffer, Buffer.Length);
            return ptr;
        }

        /// <summary>
        ///     Get a pointer to the Windows-1252 encoded string.
        ///     It will create a null-terminating string...
        /// </summary>
        public static unsafe Byte* ToPointer(this String Str, Byte* ptr)
        {
            byte[] Buffer = Encoding.GetEncoding(1252).GetBytes(Str + "\0");
            fixed (Byte* pBuffer = Buffer)
                memcpy(ptr, pBuffer, Buffer.Length);
            return ptr;
        }
    }
}
