using Ideassoccer.BaseStation.UI.Models;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

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
            await Send(destId, Encoding.UTF8.GetString(message));
        }

        public async Task Send(string destId, string message)
        {
            byte[] msg = Encoding.UTF8.GetBytes(message);

            {
                Robot robot;

                if (_robots.TryGetValue(destId, out robot!))
                {
                    await _udp.Send(robot.IPEndPoint, msg);
                    robot.Packets.Push(new Packet(DateTime.Now, PacketType.Send, msg));
                    return;
                }
            }

            foreach (var robot in _robots)
            {
                robot.Value.Packets.Push(new Packet(DateTime.Now, PacketType.Send, msg));
                _ = _udp.Send(robot.Value.IPEndPoint, msg);
            }
        }
    }
}
