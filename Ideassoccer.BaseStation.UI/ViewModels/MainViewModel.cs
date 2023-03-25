using Ideassoccer.BaseStation.UI.Utilities;
using System;
using System.Net;
using System.Collections.Generic;
using Ideassoccer.BaseStation.UI.Models;
using System.Windows.Input;
using System.Windows;

namespace Ideassoccer.BaseStation.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ICommand ListenUdpCommand { get; set; }

        private Udp _udp;
        public RobotUdpClient UdpClient;

        private int _udpPort = 4242;
        public int UdpPort
        {
            get => _udpPort;
            set => RaisePropertyChanged(ref _udpPort, value);
        }

        private Robot _robot1;
        public Robot Robot1
        {
            get => _robot1;
            set => RaisePropertyChanged(ref _robot1, value);
        }

        private Robot _robot2;
        public Robot Robot2
        {
            get => _robot2;
            set => RaisePropertyChanged(ref _robot2, value);
        }

        private RobotViewModel _robot1vm;
        public RobotViewModel Robot1VM
        {
            get => _robot1vm;
            set => RaisePropertyChanged(ref _robot1vm, value);
        }

        private RobotViewModel _robot2vm;
        public RobotViewModel Robot2VM
        {
            get => _robot2vm;
            set => RaisePropertyChanged(ref _robot2vm, value);
        }

        private Dictionary<string, string> _cbItems;
        public Dictionary<string, string> CbItems
        {
            get => _cbItems;
            set => RaisePropertyChanged(ref _cbItems, value);
        }

        private BaseStationViewModel _bstavm;
        public BaseStationViewModel BstaVM
        {
            get => _bstavm;
            set => RaisePropertyChanged(ref _bstavm, value);
        }

        public MainViewModel()
        {
            Mediator.Register(MediatorToken.UDPReceived, OnUdpReceived);

            _udp = new Udp(new IPEndPoint(IPAddress.Any, UdpPort));
            _udp.Received += _udp_Received;
            _robot1 = new Robot("1", "Robot 1", new IPEndPoint(IPAddress.Parse("192.168.8.150"), 4242));
            _robot2 = new Robot("2", "Robot 2", new IPEndPoint(IPAddress.Parse("192.168.8.151"), 4242));

            UdpClient = new RobotUdpClient(_udp, new Robots
            {
                { Robot1.Id, Robot1 },
                { Robot2.Id, Robot2 },
            });

            _robot1vm = new RobotViewModel(_robot1, 4991);
            _robot2vm = new RobotViewModel(_robot2, 4992);

            _cbItems = new Dictionary<string, string>
            {
                { "0", "All"},
                {_robot1.Id, _robot1.Name },
                {_robot2.Id, _robot2.Name },
            };
            _bstavm = new BaseStationViewModel(UdpClient, CbItems);

            ListenUdpCommand = new Command(() => { _ = _udp.Listen(); });
        }

        private void _udp_Received(object? sender, ReceivedEventArgs e)
        {
            Mediator.NotifyColleagues(MediatorToken.UDPReceived, e);
        }

        private void OnUdpReceived(object evt)
        {
            if (evt is ReceivedEventArgs e)
            {
                IPEndPoint from = e.From;

                // If packet is coming from robot1 then forward to robot2
                if (from.Equals(Robot1.IPEndPoint))
                {
                    Robot1.Packets.Push(new Packet(DateTime.Now, PacketType.Recv, e.Bytes));
                    _ = UdpClient.Send(Robot2.Id, e.Bytes);
                }
                // If packet is coming from robot2 then forward to robot1 
                else if (from.Equals(Robot2.IPEndPoint))
                {
                    Robot2.Packets.Push(new Packet(DateTime.Now, PacketType.Recv, e.Bytes));
                    _ = UdpClient.Send(Robot1.Id, e.Bytes);
                }
            }
        }
    }
}
