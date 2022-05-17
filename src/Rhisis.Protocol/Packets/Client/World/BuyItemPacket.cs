﻿using Rhisis.Abstractions.Protocol;

namespace Rhisis.Protocol.Packets.Client.World
{
    public class BuyItemPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the tab.
        /// </summary>
        public byte Tab { get; private set; }

        /// <summary>
        /// Gets the slot of the item.
        /// </summary>
        public byte Slot { get; private set; }

        /// <summary>
        /// Gets the quantity of items.
        /// </summary>
        public short Quantity { get; private set; }

        /// <summary>
        /// Gets the Item Id.
        /// </summary>
        public int ItemId { get; private set; }

        /// <inheritdoc />
        public void Deserialize(IFFPacket packet)
        {
            Tab = packet.ReadByte();
            Slot = packet.ReadByte();
            Quantity = packet.ReadInt16();
            ItemId = packet.ReadInt32();
        }
    }
}