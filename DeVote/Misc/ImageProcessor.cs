using System.Text;
using System.IO;
using System.Collections.Generic;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using ImageProcessor.Plugins.WebP.Imaging.Formats;

namespace DeVote.Misc
{
    class ImageProcessor
    {
        /// <summary>
        /// Compresses a byte[] image using ImageProcessor library.
        /// </summary>
        /// <param name="image">a byte[] image to be compressed.</param>
        /// <param name="quality">percentage to alter the image quality, defaulting to 75.</param> 
        /// <param name="outputFormat">output format defaulting to .jpg</param> 
        /// <returns>byte[] of compressed image.</returns> 
        public static byte[] Compress(byte[] image, int quality = 75, string outputFormat = "jpg")
        {
            using MemoryStream compressedImageStream = new();
            using ImageFactory imageFactory = new(preserveExifData: false);
            dynamic format = new WebPFormat();
            if (outputFormat == "png") format = new PngFormat();
            if (outputFormat == "jpg") format = new JpegFormat();

            // Save the compressed image to the specified stream.
            imageFactory.Load(image).Format(format).Quality(quality).Save(compressedImageStream);
            return compressedImageStream.ToArray();
        }

        /// <summary>
        /// Decompresses a byte[] image using ImageProcessor library and saves it in the specified path.
        /// </summary>
        /// <param name="image">image byte[] to be decompressed</param>
        /// <param name="outputPath">the output path to save the image in</param>
        public static void Decompress(byte[] image, string outputPath)
        {
            File.WriteAllBytes(outputPath, image);
        }

        /// <summary>
        ///  Compress a list of byte[] images using ImageProcessor library.
        /// </summary>
        /// <param name="images">Images to compress presented in byte arrays</param>
        /// <param name="quality">percentage to alter the image quality, defaulting to 75.</param> 
        /// <param name="outputFormat">output format defaulting to .jpg</param> 
        /// <returns>List of byte[] of compressed image.</returns>s
        public static byte[][] CompressImages(byte[][] images, int quality = 75, string outputFormat = "jpg")
        {
            byte[][] compressedImages = new byte[images.Length][];
            for (var i = 0; i < images.Length; i++)
            {
                compressedImages[i] = Compress(images[i], quality, outputFormat);
            }
            return compressedImages;
        }

        /// <summary>
        /// Decompresses a list of byte[] images and saves the images as temp files defaulting to .jpg.
        /// </summary>
        /// <returns>Temp images paths</returns>
        public static IEnumerable<string> DecompressImages(byte[][] Images, string Hash, string format = "jpg")
        {
            for (var i = 0; i < Images.Length; i++)
            {
                var image = Images[i];
                var outputPath = Path.GetTempPath() + $"{Hash}{i}.{format}";
                Decompress(image, outputPath);
                yield return outputPath;
            }
        }

        /// <summary>
        /// Decompresses the front and back of the recieved compressed ID.
        /// </summary>
        public static void DecompressID(byte[] front, byte[] back, byte[] hash, string outputformat, out string frontIDPath, out string backIDPath)
        {
            string HashString = Encoding.UTF8.GetString(hash);
            frontIDPath = Path.GetTempPath() + $"{HashString}front.{outputformat}";
            backIDPath = Path.GetTempPath() + $"{HashString}back.{outputformat}";
            Decompress(front, frontIDPath);
            Decompress(back, backIDPath);
        }
    }
}