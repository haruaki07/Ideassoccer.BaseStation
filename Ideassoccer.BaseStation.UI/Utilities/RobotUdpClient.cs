using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Robots = System.Collections.Generic.Dictionary<string, Ideassoccer.BaseStation.UI.Robot>;

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

        public async Task Send(string destId, string message)
        {
            byte[] msg = Encoding.UTF8.GetBytes(message);

            {
                Robot robot;

                if (_robots.TryGetValue(destId, out robot!))
                {
                    await _udp.Send(robot.IPEndPoint, msg);
                    return;
                }
            }

            var tasks = new List<Task>();

            foreach (var robot in _robots)
            {
                var task = new Task(async () => await _udp.Send(robot.Value.IPEndPoint, msg));
                task.Start();
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }
    }
}
