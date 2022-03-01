using DeVote.Extensions;
using DeVote.Logging;
using DeVote.Network.Handling;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DeVote.Network
{
    class PacketsHandler
    {
        /// <summary>
        /// Queue of Packets to be Processed
        /// </summary>
        public static ConcurrentQueue<KeyValuePair<Node, byte[]>> Packets = new ConcurrentQueue<KeyValuePair<Node, byte[]>>();

        /// <summary>
        /// Handlers that will handle packets
        /// </summary>
        static readonly ConcurrentDictionary<short, PacketHandler<IPacket>> _handlers = new ConcurrentDictionary<short, PacketHandler<IPacket>>();

        /// <summary>
        /// Log proxy to log events (infos, errors, warnings, etc..)
        /// </summary>
        static readonly LogProxy Log = new LogProxy("PacketsHandler");

        /// <summary>
        /// Starts the packets handling process
        /// </summary>
        /// <returns></returns>
        public static async Task Handle()
        {
            while (true)
            {
                try
                {
                    while (Packets.TryDequeue(out KeyValuePair<Node, byte[]> pair))
                    {
                        try
                        {
                            var node = pair.Key;
                            var packet = pair.Value;
                            if (IsItECDHResponse(packet)) // Already handled before the Handler starts to work, so just dequeue
                                continue; 
                            else if (IsItECDHRequest(packet)) // Check if it's Elliptic Curve Diffie Hellman Key Exchange packet
                            {
                                // Extract other party public key from the packet (we skip our header/identifier)
                                var otherPartyPublicKey = packet.Skip(Constants.ECDHOperations[0].Length).ToArray();

                                // Send an inital response containing our public key (we add an inital response header to the beginning of the packet as well)
                                node.Send(Constants.ECDHOperations[1].Concat(Cryptography.ECDH.PublicKey.ToByteArray()).ToArray(), false);

                                // We seraialize our AES Key (Key and IV) using ProtoBuf
                                byte[] serializedAesKey;
                                using (var ms = new System.IO.MemoryStream())
                                {
                                    ProtoBuf.Serializer.Serialize(ms, Cryptography.AES.Key);
                                    serializedAesKey = ms.ToArray();
                                }
                                // We then encrypt it using Elliptic Curve Diffie Hellman Cryptography
                                var encryptedAESKey = Cryptography.ECDH.Encrypt(serializedAesKey, otherPartyPublicKey, out byte[] IV);

                                // Send the final response containing: Encrypted serialized AES Key, ECDH IV, and at the beginning of course the header
                                node.Send(Constants.ECDHOperations[2].Concat(encryptedAESKey.Concat(IV)).ToArray(), false);
                            }
                            else if (Cryptography.AES.Key != null) // If it's not an ECDH packet and we have our AES Key, then we handle it 
                            {
                                packet = Cryptography.AES.Decrypt(packet); // Decrypt the packet using our AES Key

                                // Our Header containing the ID at the first 2 bytes (0 and 1), and the length at the next 2 bytes (2 and 3)
                                var id = BitConverter.ToInt16(packet, 0);
                                var length = BitConverter.ToInt16(packet, 2);

                                if (packet.Length - 4 == length) // Verify that the length is accurate (we deduct header length)
                                {
                                    try
                                    {
                                        // Check if that packet can be handled
                                        if (_handlers.ContainsKey(id))
                                            _handlers[id].Invoke(node, packet); // Handle the packet
                                        else Log.Error($"Unhandled packet with ID: {id}"); // Notify that this packet is not handled
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Error($"Exception occured during handling packet: {id}, message: {e.Message}");
                                    }
                                }
                                else // Probably a corrupted packet
                                    Log.Error($"Invalid packet length, expected: {length} actual: {packet.Length - 4}");
                            }
                            else // If we don't have the AES Key, and its not an ECDH packet, ignore it for now, add it to the end of queue again
                            {
                                Packets.Enqueue(pair);
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error($"Exception occured during handling a packet, message: {e.Message}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Exception occured during packets dequeuing, message: {e.Message}");
                }
            }
        }
        /// <summary>
        /// Checks if the packet contains an ECDH Request
        /// </summary>
        /// <param name="packet">the packet to be checked</param>
        /// <returns></returns>
        static bool IsItECDHRequest(byte[] packet)
        {
            return packet.StartsWith(Constants.ECDHOperations[0]);
        }
        /// <summary>
        /// Checks if the packet contains an ECDH Response
        /// </summary>
        /// <param name="packet">the packet to be checked</param>
        /// <returns></returns>
        static bool IsItECDHResponse(byte[] packet)
        {
            return packet.StartsWith(Constants.ECDHOperations[1]) || packet.StartsWith(Constants.ECDHOperations[2]);
        }
        /// <summary>
        /// Initalize the Packet Handler, by catching all handlers by a special attribute (NodePacketHandlerAttribute) using Reflection
        /// </summary>
        public static void Init()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                CacheHandlers(asm);
            Log.Info("{0} Packet Handler classes cached.", _handlers.Count);
        }
        /// <summary>
        /// Catches all handlers by a special attribute (NodePacketHandlerAttribute) using Reflection
        /// </summary>
        /// <param name="asm">Assembly to catch handlers from</param>
        static void CacheHandlers(Assembly asm)
        {
            foreach (var type in asm.GetTypes())
            {
                var attr = ReflectionExtensions.GetCustomAttribute<NodePacketHandlerAttribute>(type);
                if (attr == null) continue;

                var parent = typeof(IPacket);
                if (!type.IsAssignableTo(parent))
                {
                    Log.Error("{0} classes must inherit from {1}", attr, parent);
                    continue;
                }

                var ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null,
                    new[] { typeof(byte[]) }, null);
                if (ctor == null)
                {
                    Log.Error("{0} needs a public instance constructor with one type argument as a byte array.",
                        type.Name);
                    continue;
                }

                var typeId = attr.PacketTypeId;
                var handler = new PacketHandler<IPacket>(ctor, typeId);

                _handlers.Add(key: ((IConvertible)typeId).ToInt16(null), value: handler);
            }
        }
    }
}
