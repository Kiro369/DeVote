using System;
using System.IO;
using System.Text;
using DeVote.Misc;
using DeVote.PyRecognition;
using ProtoBuf;

namespace DeVote.Structures
{
    [ProtoContract(SkipConstructor = true)]
    class TransactionRecord
    {
        [ProtoMember(1)] public byte[] Hash;
        [ProtoMember(2)] public byte[] Front { get; set; }
        [ProtoMember(3)] public byte[] Back { get; set; }
        [ProtoMember(4)] public byte[][] Images { get; set; }


        public TransactionRecord(byte[] hash, byte[] front, byte[] back, byte[][] images)
        {
            Hash = hash;
            Front = front;
            Back = back;
            Images = images;
        }


        /// <summary>
        /// Verify the face of the voter from captured images against face from ID.
        /// </summary>
        /// <param name="frontIDPath">the frontside of ID to be matched against.</param> 
        /// <returns></returns>
        public bool IsVoterVerified(string frontIDPath)
        {
            var verified = 0;
            foreach (var imagePath in Misc.ImageProcessor.DecompressImages(Images, Encoding.UTF8.GetString(Hash)))
            {
                if (Recognition.Current.VerifyVoter(frontIDPath, imagePath))
                    verified++;
            }
            return verified >= Images.Length / 2;
        }

        /// <summary>
        /// Decompresses the front and back of transaction record in the desired format defaulting to .jpg.
        /// </summary>
        /// <param name="outputformat"></param>
        /// <returns>tuple of paths where front and back is saved</returns>
        public (string,string) DecompressID(string outputformat = "jpg")
        {
            Misc.ImageProcessor.DecompressID(Front, Back, Hash, outputformat, out string frontIDPath, out string backIDPath);
            return (frontIDPath, backIDPath);
        }

        public static byte[] Serialize(TransactionRecord transactionRecord)
        {
            using var stream = new MemoryStream();
            Serializer.Serialize(stream, transactionRecord);
            return stream.ToArray();
        }

        public static TransactionRecord Deserialize(byte[] data)
        {
            using var stream = new MemoryStream(data);
            return Serializer.Deserialize<TransactionRecord>(stream);
        }
    }

}
