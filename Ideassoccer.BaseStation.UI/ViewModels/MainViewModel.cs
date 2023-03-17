using Ideassoccer.BaseStation.UI.Utilities;
using System;
using System.Net;
using System.Collections.Generic;
using Ideassoccer.BaseStation.UI.Models;

namespace Ideassoccer.BaseStation.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private Udp _udp;

        public RobotUdpClient UdpClient;
        private Robot _robot1;
        private Robot _robot2;

        public Robot Robot1
        {
            get => _robot1;
            set => RaisePropertyChanged(ref _robot1, value);
        }
        public Robot Robot2
        {
            get => _robot2;
            set => RaisePropertyChanged(ref _robot2, value);
        }

        public MainViewModel(Udp udp)
        {
            _udp = udp;
            _udp.Received += _udp_Received;
            _robot1 = new Robot("1", "Robot 1", new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4241));
            _robot2 = new Robot("2", "Robot 2", new IPEndPoint(IPAddress.Parse("192.168.8.151"), 4242));

            UdpClient = new RobotUdpClient(_udp, new Robots
            {
                { Robot1.Id, Robot1 },
                { Robot2.Id, Robot2 },
            });
        }

        private void _udp_Received(object? sender, ReceivedEventArgs e)
        {
            IPEndPoint from = e.From;

            // If packet is coming from robot1 then forward to robot2
            if (from.Address.ToString() == Robot1.IPEndPoint.Address.ToString())
            {
                Robot1.Packets.Push(new Packet(DateTime.Now, PacketType.Recv, e.Bytes));
                _ = UdpClient.Send(Robot2.Id, e.Bytes);
            }
            // If packet is coming from robot2 then forward to robot1 
            else if (from.Address.ToString() == Robot2.IPEndPoint.Address.ToString())
            {
                Robot2.Packets.Push(new Packet(DateTime.Now, PacketType.Recv, e.Bytes));
                _ = UdpClient.Send(Robot1.Id, e.Bytes);
            }
        }
    }
}
