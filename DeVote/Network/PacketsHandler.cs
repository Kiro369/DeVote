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
        public static ConcurrentQueue<KeyValuePair<Node, byte[]>> Packets = new ConcurrentQueue<KeyValuePair<Node, byte[]>>();
        static readonly ConcurrentDictionary<short, PacketHandler<IPacket>> _handlers = new ConcurrentDictionary<short, PacketHandler<IPacket>>();
        static readonly LogProxy Log = new LogProxy("PacketsHandler");

        public static async Task Handle()
        {
            while (true)
            {
                while (Packets.TryDequeue(out KeyValuePair<Node, byte[]> pair))
                {
                    var node = pair.Key;
                    var packet = pair.Value;
                    if (IsItECDHPacket(packet))
                    {
                        var otherPartyPublicKey = packet.Skip(Program.ECDHOperations[0].Length).ToArray();
                        node.Send(Program.ECDHOperations[1].Concat(Cryptography.ECDH.PublicKey.ToByteArray()).ToArray());
                        byte[] serializedAesKey;
                        using (var ms = new System.IO.MemoryStream())
                        {
                            ProtoBuf.Serializer.Serialize(ms, Cryptography.AES.Key);
                            serializedAesKey = ms.ToArray();
                        }
                        var encryptedAESKey = Cryptography.ECDH.Encrypt(serializedAesKey, otherPartyPublicKey, out byte[] IV);
                        node.Send(Program.ECDHOperations[2].Concat(encryptedAESKey.Concat(IV)).ToArray());
                    }
                    else
                    {
                        _handlers[0].Invoke(node, packet);
                    }
                }
            }
        }
        static bool IsItECDHPacket(byte[] packet)
        {
            return packet.StartsWith(Program.ECDHOperations[0]);
        }
        public static void Init()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                CacheHandlers(asm);
            Log.Info("{0} Packet Handler classes cached.", _handlers.Count);
        }
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
                    new[] { typeof(Byte[]) }, null);
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
