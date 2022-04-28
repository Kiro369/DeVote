using DeVote.Cryptography;
using DeVote.Extensions;
using DeVote.Network.Transmission;
using DeVote.PyRecognition;
using DeVote.Structures;
using ProtoBuf;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace DeVote.Network.Messages
{
    [ProtoContract(SkipConstructor = true)]
    [Handling.NodePacketHandler(PacketTypes.Transaction)]
    class Transaction : Packet
    {
        [ProtoMember(1)] public Structures.Transaction TransactionData { get; set; }
        [ProtoMember(2)] (byte[] Front, byte[] Back) ID { get; set; }
        [ProtoMember(3)] public byte[][] Images { get; set; }

        public Transaction(byte[] incomingPacket) : base(incomingPacket) {}

        public override void Handle(Node client)
        {
            if (VotedDLT.Current.Contains(TransactionData.Elector))
            {
                // TODO: Boradcast a warning that this voter did vote before
            }
            else
            {
                DecompressID(out string frontIDPath, out string backIDPath);
                var verified = 0;
                foreach (var imagePath in DecompressImages())
                {
                    if (Recognition.Current.VerifyVoter(frontIDPath, imagePath))
                        verified++;
                }
                bool voterVerified = verified >= Images.Length / 2;
                if (voterVerified)
                {
                    var extractInfo = Recognition.Current.ExtractIDInfo(frontIDPath, "front");
                    if (extractInfo != null)
                    {
                        var extractedID = (string)extractInfo.ID;
                        if (string.IsNullOrEmpty(extractedID))
                        {
                            var hash = Argon2.ComputeHash(extractedID);
                            if (hash.Equals(TransactionData.Elector)) 
                            {
                                VotedDLT.Current.Add(TransactionData.Elector, client.MachineID);
                                TransactionData.Confirmations++;
                                TransactionData.Elector = client.MachineID;
                                Blockchain.Current.Block.AddTransaction(TransactionData);

                                // TODO: Send confirmation
                            }
                            else
                            {
                                // TODO: Hash didn't match the supplied hash, this transaction is compromised, or maybe its the machine????! 
                            }
                        }
                        else
                        {
                            // TODO: Acknowledge that we couldn't extract the ID number from the supplied ID image
                        }
                    }
                    else
                    {
                        // TODO: Acknowledge that we couldn't verify extract ID info from the supplied ID
                    }
                }
                else
                {
                    // TODO: Acknowledge that we couldn't verify this voter 
                }
            }
        }

        public override bool Read(Node client)
        {
            try
            {
                Deserialize<Transaction>().CopyProperties(this);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Decompresses the front and back of the recieved compressed ID
        /// </summary>
        private void DecompressID(out string frontIDPath, out string backIDPath)
        {
            frontIDPath = Path.GetTempPath() + $"{TransactionData.Hash}front.png";
            backIDPath = Path.GetTempPath() + $"{TransactionData.Hash}back.png";
            Decompress(ID.Front, frontIDPath);
            Decompress(ID.Back, backIDPath);
        }

        /// <summary>
        /// Deflate compression for our PNGs
        /// </summary>
        /// <param name="images">Images to compress presented in byte arrays</param>
        public void CompressImages(params byte[][] images)
        {
            Images = new byte[images.Length][];
            for (var i = 0; i < images.Length; i++)
            {
                var image = images[i];
                Images[i] = Compress(image);
            }
        }

        /// <summary>
        /// Compresses a byte[] image using Deflate compression algorithm
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public byte[] Compress(byte[] image)
        {
            using var originalImageStream = new MemoryStream(image);
            using var compressedImageStream = new MemoryStream();
            using var compressor = new DeflateStream(compressedImageStream, CompressionMode.Compress);
            originalImageStream.CopyTo(compressor);
            return compressedImageStream.ToArray();
        }

        /// <summary>
        /// Decompresses the images recieved from the Deflate compression and saves the images as temp files
        /// </summary>
        /// <returns>Temp images paths</returns>
        public IEnumerable<string> DecompressImages()
        {
            for (var i = 0; i < Images.Length; i++)
            {
                var image = Images[i];
                var outputPath = Path.GetTempPath() + $"{TransactionData.Hash}{i}.png";
                Decompress(image, outputPath);
                yield return outputPath;
            }
        }

        /// <summary>
        /// Decompresses a byte[] image using Deflate compression algorithm and saves it to the specified path
        /// </summary>
        /// <param name="image">image byte[] to be decompressed</param>
        /// <param name="outputPath">the output path to save the image in</param>
        public void Decompress(byte[] image, string outputPath)
        {
            using var compressedImageStream = new MemoryStream(image);
            using FileStream outputFileStream = File.Create(outputPath);
            using var decompressor = new DeflateStream(compressedImageStream, CompressionMode.Decompress);
            decompressor.CopyTo(outputFileStream);
        }
    }
}
