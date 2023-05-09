using Ideassoccer.BaseStation.UI.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Ideassoccer.BaseStation.UI.Utilities
{

    public class RobotUdpClient
    {
        private Udp _udp;
        private Robots _robots;

        public RobotUdpClient(Udp udp, Robots robots)
        {
            _udp = udp;
            _robots = robots;
        }

        public async Task Send(string destId, byte[] message)
        {
            {
                Robot robot;

                if (_robots.TryGetValue(destId, out robot!))
                {
                    await _udp.Send(robot.IPEndPoint, message);
                    robot.Packets.Push(new Packet(DateTime.Now, PacketType.Send, message));
                    return;
                }
            }

            foreach (var robot in _robots)
            {
                robot.Value.Packets.Push(new Packet(DateTime.Now, PacketType.Send, message));
                _ = _udp.Send(robot.Value.IPEndPoint, message);
            }
        }

        //public async Task Send(string destId, RobotMessage message)
        //{
        //    await Send(destId, FormatRobotMessage(message));
        //}

        public async Task Send(string destId, string message)
        {
            await Send(destId, Encoding.UTF8.GetBytes(message));
        }

        //private string FormatRobotMessage(RobotMessage message)
        //{
        //    return string.Format(
        //        "{0},{1},{2},{3},{4}",
        //        message.Command,
        //        message.Kes,
        //        message.X,
        //        message.Y,
        //        message.Z
        //    );
        //}
    }
}
