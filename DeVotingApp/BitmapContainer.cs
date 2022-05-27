using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeVotingApp
{
    public class BitmapContainer
    {
        public PixelFormat Format { get; }

        public int Width { get; }

        public int Height { get; }

        public IntPtr Buffer { get; }

        public int Stride { get; set; }

        public BitmapContainer(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            Format = bitmap.PixelFormat;
            Width = bitmap.Width;
            Height = bitmap.Height;

            var bufferAndStride = ToBufferAndStride(bitmap);
            Buffer = bufferAndStride.Item1;
            Stride = bufferAndStride.Item2;
        }
        Tuple<IntPtr, int> ToBufferAndStride(Bitmap bitmap)
        {
            BitmapData bitmapData = null;

            try
            {
                bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly, bitmap.PixelFormat);

                return new Tuple<IntPtr, int>(bitmapData.Scan0, bitmapData.Stride);
            }
            finally
            {
                if (bitmapData != null)
                    bitmap.UnlockBits(bitmapData);
            }
        }
        public Bitmap ToBitmap()
        {
            return new Bitmap(Width, Height, Stride, Format, Buffer);
        }
    }
}
