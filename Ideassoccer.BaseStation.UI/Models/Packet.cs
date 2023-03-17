using System;
using System.Text;

namespace Ideassoccer.BaseStation.UI.Models
{
    public class Packet
    {
        public DateTime Time { get; set; }
        public PacketType Type { get; set; }
        public byte[] Bytes { get; set; }

        public Packet(DateTime Time, PacketType Type, byte[] bytes)
        {
            this.Time = Time;
            this.Type = Type;
            Bytes = bytes;
        }

        public string MessageString()
        {
            return Encoding.UTF8.GetString(Bytes);
        }

        public override string ToString()
        {
            return MessageString();
        }
    }


    public enum PacketType
    {
        Recv,
        Send
    }
}
