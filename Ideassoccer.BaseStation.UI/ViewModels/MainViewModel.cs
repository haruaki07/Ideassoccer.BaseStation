using Ideassoccer.BaseStation.UI.Models;
using Ideassoccer.BaseStation.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Windows.Input;

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

        private PositionViewModel _posvm;
        public PositionViewModel PosVM
        {
            get => _posvm;
            set => RaisePropertyChanged(ref _posvm, value);
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
            _robot1 = new Robot("1", "Robot 1", new IPEndPoint(IPAddress.Parse("192.168.8.150"), 4242), null);
            _robot2 = new Robot("2", "Robot 2", new IPEndPoint(IPAddress.Parse("192.168.8.151"), 4242), null);

            UdpClient = new RobotUdpClient(_udp, new Robots
            {
                { Robot1.Id, Robot1 },
                { Robot2.Id, Robot2 },
            });

            _robot1vm = new RobotViewModel(_robot1, 4991);
            _robot2vm = new RobotViewModel(_robot2, 4992);

            _posvm = new PositionViewModel(Robot1, Robot2);

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
                    if (e.Bytes[0] == 'x')
                    {
                        float[] pos = parsePosition(e.Bytes);
                        Mediator.NotifyColleagues(MediatorToken.Robot1Moved, new Position(pos[0], pos[1]));
                    }

                    _ = UdpClient.Send(Robot2.Id, e.Bytes);
                }
                // If packet is coming from robot2 then forward to robot1 
                else if (from.Equals(Robot2.IPEndPoint))
                {
                    Robot2.Packets.Push(new Packet(DateTime.Now, PacketType.Recv, e.Bytes));
                    if (e.Bytes[0] == 'x')
                    {
                        float[] pos = parsePosition(e.Bytes);
                        Mediator.NotifyColleagues(MediatorToken.Robot2Moved, new Position(pos[0], pos[1]));
                    }

                    _ = UdpClient.Send(Robot1.Id, e.Bytes);
                }
            }
        }

        /// <summary>
        /// Parse position format ('x200,100')
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        float[] parsePosition(byte[] bytes)
        {
            float[] ret = { 0, 0 };

            string payload = Encoding.UTF8.GetString(bytes)[1..].ToString();
            string[] payloadArr = payload.Split(',');

            if (payloadArr.Length == 2)
            {
                ret[0] = float.Parse(payloadArr[0], CultureInfo.InvariantCulture);
                ret[1] = float.Parse(payloadArr[1], CultureInfo.InvariantCulture);
            }


            return ret;
        }
    }
}
