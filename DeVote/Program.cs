using DeVote.Cryptography;
using DeVote.Extensions;
using DeVote.Network;
using DeVote.Network.Communication;
using DeVote.Structures;
using DeVote.PyRecognition;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text;
using DeVote.Misc;

namespace DeVote
{
    class Program
    {
        static void Main()
        {
            #region TransactionDLT/Record Test
            var pkt = new List<byte>();
            pkt.AddRange(BitConverter.GetBytes((short)5));
            pkt.AddRange(BitConverter.GetBytes((short)1004));
            pkt.Add(0);
            pkt.AddRange(BitConverter.GetBytes(50));

            var x = Blockchain.Current.LevelDB.Handle;
            Blockchain.Current.LevelDB.Close();
            var y = Blockchain.Current.LevelDB.Handle == IntPtr.Zero;
            Console.ReadLine();
            //TransactionRecord transactionRecord = new();
            //string frontImage = @"M:\GradutaionProject\sisi\front.jpg";
            //string backImage = @"M:\GradutaionProject\sisi\back.jpg";
            //string frameDir = @"M:\GradutaionProject\sisi\sisiFrames";

            //string[] frameImages = Directory.GetFiles(frameDir);

            //var fBitmap = File.ReadAllBytes(frontImage); 
            //var bBitmap = File.ReadAllBytes(backImage);

            //var fcompressed = Misc.ImageProcessor.Compress(fBitmap, 55, "webp");
            //var bcompressed = Misc.ImageProcessor.Compress(bBitmap, 55, "webp");

            //Misc.ImageProcessor.Decompress(fcompressed, @"M:\GradutaionProject\sisi\webp\Coutput3.webp");
            //Misc.ImageProcessor.Decompress(bcompressed, @"M:\GradutaionProject\sisi\webp\Coutput3.jpg");

            //transactionRecord.Hash = Encoding.UTF8.GetBytes("F644B7333382BBD4C6948FEEABAFD03649A89D93DE651D351A2FFD6283F6824B");
            //transactionRecord.Front = fcompressed;
            //transactionRecord.Back = bcompressed;

            //(string frontIDPath, string bacjIDPath) = transactionRecord.DecompressID("webp");

            //List<byte[]> images = new() { };

            //foreach (string frameImage in frameImages)
            //{
            //    images.Add(Misc.ImageProcessor.Compress(File.ReadAllBytes(frameImage), 55, "jpg"));
            //};

            //transactionRecord.Images = images.ToArray();

            //Misc.ImageProcessor.DecompressImages(transactionRecord.Images, Encoding.UTF8.GetString(transactionRecord.Hash), "webp");


            //var stopWatch2 = new System.Diagnostics.Stopwatch(); stopWatch2.Start();
            //bool IsVerified = transactionRecord.IsVoterVerified(frontImage);
            //stopWatch2.Stop();

            //Console.WriteLine($"total time {stopWatch2.ElapsedMilliseconds}");
            //Console.WriteLine($"IsVerified {IsVerified}");

            //TransactionsDLT.Current.AddRecord(transactionRecord);

            //TransactionRecord DeserializedTransactionRecord = TransactionsDLT.Current.GetRecord(transactionRecord.Hash);

            //Console.WriteLine($"Og Hash {Encoding.UTF8.GetString(transactionRecord.Hash)}");
            //Console.WriteLine($"De Hash {Encoding.UTF8.GetString(DeserializedTransactionRecord.Hash)}");


            //for (int i = 0; i < DeserializedTransactionRecord.Images.Length; i++)
            //{
            //    var dir = @"M:\GradutaionProject\sisi\sisiFramesGen\";
            //    File.WriteAllBytes(dir + i + ".jpg", DeserializedTransactionRecord.Images[i]);
            //}

            //#endregion
            ////#region Compression test
            //var stopWatch = new System.Diagnostics.Stopwatch(); stopWatch.Start();
            //var image = File.ReadAllBytes(@"M:\GradutaionProject\sisi\front.jpg");
            //var compressed = Misc.ImageProcessor.Compress(image, 55, "jpg");
            //stopWatch.Stop();
            //Console.WriteLine($"Compressed size from {image.Length} to {compressed.Length} => {1 - compressed.Length / (double)image.Length}% reduced => in {stopWatch.ElapsedMilliseconds}");

            //stopWatch.Restart();
            //File.WriteAllBytes(@"M:\GradutaionProject\sisi\realoutput2.webp", compressed);
            //Misc.ImageProcessor.Decompress(compressed, @"M:\GradutaionProject\sisi\output2.jpg");
            //Misc.ImageProcessor.Decompress(compressed, @"M:\GradutaionProject\sisi\output2.webp");
            //stopWatch.Stop();
            //Console.WriteLine($"Time to decompress: {stopWatch.ElapsedMilliseconds}");
            //Console.ReadLine();
            return;

            #endregion
            //#region Settings Test
            //Console.WriteLine(Settings.Current.Argon2Salt);
            //Console.WriteLine(Settings.Current.BlockchainPath);
            //Console.WriteLine(Settings.Current.VotedDLTPath);
            //Console.WriteLine(Settings.Current.PythonDLLPath);
            //Console.WriteLine(Settings.Current.Latitude);
            //Console.WriteLine(Settings.Current.Longitude);
            //Console.WriteLine(Settings.Current.BlockchainExplorerPort);

            //NetworkManager.SendLocation(true);
            //return;

            //#endregion

            #region Embedding Python in .Net Test

            ////var testID = "1020304050";
            ////var votedDLT = new VotedDLT();
            //////votedDLT.Add(testID);

            ////var voted = votedDLT.Contains(testID);
            ////var voted2 = votedDLT.Contains("5040302010");

            ////var test = new Network.Messages.Test(null);
            ////var packet = test.Create("Testing new packet stuff");

            ////var dePacket = new Network.Messages.Test(packet);
            ////dePacket.Read(null);

            //var imgPath = @"C:\Users\Kiro\Desktop\cards\real-back5.jpg";

            //dynamic IDInfo = Recognition.Current.ContainsCard(imgPath);
            //Console.WriteLine($"IDInfo {IDInfo}");


            ////string cardsDirectory = @"M:\GradutaionProject\barcode Test\back set\cards";
            ////string[] cardImages = Directory.GetFiles(cardsDirectory);
            ////foreach (string cardImage in cardImages)
            ////{
            ////    Console.WriteLine($"Image: {cardImage}");
            ////    dynamic isCardPresent = Recognition.Current.ContainsCard(cardImage);
            ////    Console.WriteLine($"isCardPresent {isCardPresent}");
            ////}

            ////return;

            ////string backDirectory = @"M:\GradutaionProject\barcode Test\back set\compressed-full-back";
            ////string[] backImages = Directory.GetFiles(backDirectory);
            ////foreach (string image in backImages)
            ////{
            ////    Console.WriteLine($"Image: {image}");
            ////    Stopwatch stopWatch = new Stopwatch();
            ////    stopWatch.Start();
            ////    bool isIdSideBackAPI = Recognition.Current.IsIdSideABackAPI(image);
            ////    stopWatch.Stop();

            ////    long ms = stopWatch.ElapsedMilliseconds;
            ////    Console.WriteLine($"Sending and getting respone took {ms}ms - {ms / 1000}s");
            ////    Console.WriteLine($"isIdSideBack: {isIdSideBackAPI}\n");
            ////}

            ////string[] framePaths = { @"M:\GradutaionProject\Backend\frame1.jpg", @"M:\GradutaionProject\Backend\frame2.jpg" };
            ////for (int i = 0; i < framePaths.Length; i++)
            ////{
            ////    bool isVerified = Recognition.Current.VerifyVoter(imgPath, framePaths[i]);
            ////    Console.WriteLine($"is frame {i} Verified ? {isVerified}");
            ////}

            //////recognition.EndPythonInterpreter();
            //return;
            #endregion

            //#region Test
            //#endregion
            #region Main
            // Start the server first, so anyone can connect after we get added to the seeder
            var server = new Server(4269);
            server.RunServerAsync();

            //Initialize Packet Handler
            PacketsHandler.Init();

            // Starting the seeder client to be able to connect to the network
            DNSSeeder.AsynchronousClient seederClient = new();

            // Get the addresses of all nodes, and add out IP to the seeder
            seederClient.StartClient(server.Port);

            // Check if there is anyone on network, if there is none, we create our own AES Key
            if (seederClient.EndPoints.Count == 0)
            {
                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.GenerateKey();
                    aes.GenerateIV();
                    AES.Key = new AESKey()
                    {
                        Key = aes.Key,
                        IV = aes.IV
                    };
                }
                // Start the packet Handler since we have our AES Key, we can Decrypt incoming packets from the network
                Task.Factory.StartNew(PacketsHandler.Handle);
            }
            else
            {
                // If there is someone on the network, we connect to them, and we do a Key Exchange with the first one we connect to
                seederClient.EndPoints.ForEach(endPoint => Task.Factory.StartNew(() => { new Node(endPoint).Start(); }));

                // Wait till we connect
                while (NetworkManager.NodesCount == 0) Task.Delay(100);

                // Get the connection
                var node = NetworkManager.GetNodes().FirstOrDefault();

                // Generate request by adding special request bytes to the beginning of the packet (so the other side can Identify the packet), and adding our public key
                var request = Constants.ECDHOperations[0].Concat(ECDH.PublicKey).ToArray();

                // Send the request
                node.Send(request);

                // Wait for an inital response containing other party public key
                while (!PacketsHandler.Packets.Any(y => y.Value.Take(Constants.ECDHOperations[1].Length).SequenceEqual(Constants.ECDHOperations[1]))) ;

                // Get that response
                var responsePacket = PacketsHandler.Packets.First(y => y.Value.Take(Constants.ECDHOperations[1].Length).SequenceEqual(Constants.ECDHOperations[1])).Value;

                // Extract other party public key
                var otherPartyPublicKey = responsePacket.Skip(Constants.ECDHOperations[1].Length).ToArray();

                // Wait for the final response, containing the secret AES Key 
                while (!PacketsHandler.Packets.Any(y => y.Value.Take(Constants.ECDHOperations[2].Length).SequenceEqual(Constants.ECDHOperations[2]))) ;

                // Get that response
                responsePacket = PacketsHandler.Packets.First(y => y.Value.Take(Constants.ECDHOperations[2].Length).SequenceEqual(Constants.ECDHOperations[2])).Value.Skip(Constants.ECDHOperations[2].Length).ToArray();

                // Split the encrypted and serialized Key from ECDH AES IV 
                responsePacket.Split(responsePacket.Length - 16, out byte[] encryptedSecretKey, out byte[] IV);

                // Decrypt the Key
                var aesKeySerialized = ECDH.Decrypt(encryptedSecretKey, otherPartyPublicKey, IV);

                // Finally Deserialize the AES Key
                var key = ProtoBuf.Serializer.Deserialize<AESKey>(new System.Buffers.ReadOnlySequence<byte>(aesKeySerialized));

                // Set the Key, so we can use it
                AES.Key = key;

                NetworkManager.Sync();

                // Start the packet Handler since we have now our AES Key, we can Decrypt incoming packets from the network
                Task.Factory.StartNew(PacketsHandler.Handle);

                // Tell the network our MachineID and if we're wether a FullNode or Not
                NetworkManager.Broadcast(new Network.Messages.LRNConsensus().Create(long.MaxValue, Settings.Current.FullNode));
            }

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var lines = NetworkManager.GetNodes().Select(node => node.EndPoint + "=" + node.Connected + "|");
                    Console.Title = "|" + string.Join(Environment.NewLine, lines);
                    await Task.Delay(1000);
                }
            });
            while (true)
            {
                Console.WriteLine("Write a msg");
                var msg = Console.ReadLine();
                var test = new Network.Messages.Test(null);
                foreach (var node in NetworkManager.GetNodes())
                {
                    node.Send(test.Create(msg));
                }
            }
            #endregion

            #region Blockchain Test
            ////return;

            // Blockchain deVOTE = new Blockchain();

            // // load form leveldb 
            // // deVOTE.LoadStringifiedBlockchain();
            // deVOTE.LoadProtobuffedBlockchain();

            // if (deVOTE.Blocks.Count == 0)
            // {
            //     Block GenesisBlock = deVOTE.CreateGenesisBlock();
            //     deVOTE.Blocks.AddFirst(GenesisBlock);
            //     //GenesisBlock.SaveBlockAsString(deVOTE.LevelDB);
            //     GenesisBlock.SaveBlockAsByteArray(deVOTE.LevelDB);
            // }

            // Console.WriteLine("current blocks");
            // Console.WriteLine(JsonConvert.SerializeObject(deVOTE.Blocks, Formatting.Indented));


            // for (int t = 0; t < 149; t++)
            // {
            //     List<Transaction> myTransactions = new List<Transaction>();
            //     for (int i = 0; i < 4; i++)
            //     {
            //         Transaction newTX = new Transaction("elector" + i + t, "elected" + i + t);
            //         myTransactions.Add(newTX);
            //     }
            //     Block myBlock = new Block(myTransactions);
            //     myBlock.Miner = "Test33";
            //     myBlock.Transactions = myTransactions;

            //     // add new block to current blockchain
            //     deVOTE.AddBlock(myBlock);
            // }


            // // Console.WriteLine(JsonConvert.SerializeObject(myTransactions, Formatting.Indented));

            // // Save block into leveldb
            // //deVOTE.SaveBlockchainAsString();
            // deVOTE.SaveBlockchainAsByteArray();

            // Console.WriteLine("new blocks");
            // Console.WriteLine(JsonConvert.SerializeObject(deVOTE.Blocks, Formatting.Indented));


            // //load a block
            // // Block TargetBlock = Block.LoadStringifiedBlock(2, deVOTE.LevelDB);
            // Block TargetBlock = Block.LoadProtobuffedBlock(2, deVOTE.LevelDB);
            // Console.WriteLine("TargetBlock");
            // Console.WriteLine(JsonConvert.SerializeObject(TargetBlock, Formatting.Indented));

            // // load null block 
            // // Block NullBlock = Block.LoadStringifiedBlock(600, deVOTE.LevelDB);
            // Block NullBlock = Block.LoadProtobuffedBlock(600, deVOTE.LevelDB);
            // Console.WriteLine("NullBlock");
            // Console.WriteLine(JsonConvert.SerializeObject(NullBlock, Formatting.Indented));

            // // var SerializedBlockchain = deVOTE.SerializeBlockchain();
            // // Console.WriteLine("SerializedBlockchain", SerializedBlockchain);

            // // Close the connection
            // deVOTE.LevelDB.Close();
            #endregion

            #region Prev tests
            //var originalText = "hello bitches";
            //Console.WriteLine("Original text: " + originalText);
            //var originalBytes = Encoding.UTF8.GetBytes(originalText);
            //var encryptedBytes = Network.Cryptography.AES.Encrypt(originalBytes);
            //Console.WriteLine("Encrypted string: " + Encoding.UTF8.GetString(encryptedBytes));
            //var decryptedBytes = Network.Cryptography.AES.Decrypt(encryptedBytes);
            //Console.WriteLine("Decypted string: " + Encoding.UTF8.GetString(decryptedBytes));
            //Console.WriteLine("Original and Decrypted bytes match: " + originalBytes.SequenceEqual(decryptedBytes));
            //Console.ReadLine();
            //// return;
            #endregion
        }
    }
}