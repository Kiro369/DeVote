using System;
using System.Collections.Generic;
using System.Text;

namespace DeVote.Network.Handling
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class NodePacketHandlerAttribute : PacketHandlerAttribute
    {
        public NodePacketHandlerAttribute(PacketTypes typeId)
            : base(typeId)
        {
        }

        public override string ToString()
        {
            return "Node packet handler";
        }
    }
}
