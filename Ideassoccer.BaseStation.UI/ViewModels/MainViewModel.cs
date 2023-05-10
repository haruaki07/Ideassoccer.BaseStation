using Ideassoccer.BaseStation.UI.Models;
using Ideassoccer.BaseStation.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Input;

namespace Ideassoccer.BaseStation.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region propdef
        public ICommand ListenUdpCommand { get; set; }
        public ICommand GetWiFiIPCommand { get; set; }
        public ICommand CopyWiFiIPCommand { get; set; }

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

        private string? _hostIP;
        public string? HostIP { get => _hostIP; set => RaisePropertyChanged(ref _hostIP, value); }
        #endregion

        #region ctor
        public MainViewModel()
        {
            Mediator.Register(MediatorToken.UDPReceived, OnUdpReceived);
            Mediator.Register(MediatorToken.NetworkInterfaceChanged, OnNetChanged);

            _udp = new Udp(new IPEndPoint(IPAddress.Any, UdpPort));
            _udp.Received += _udp_Received;
            _robot1 = new Robot("1", "Robot 1", new IPEndPoint(IPAddress.Parse("192.168.0.100"), 4221), null);
            _robot2 = new Robot("2", "Robot 2", new IPEndPoint(IPAddress.Parse("192.168.0.103"), 4242), null);

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
            GetWiFiIPCommand = new Command(() =>
            {
                HostIP = Networking.GetWiFiIP();
                NetworkChange.NetworkAddressChanged += (s, e) =>
                    Mediator.NotifyColleagues(MediatorToken.NetworkInterfaceChanged, e);
            });
            CopyWiFiIPCommand = new Command(() =>
            {
                if (HostIP != null) Clipboard.SetText(HostIP);
            });
        }
        #endregion

        private void _udp_Received(object? sender, ReceivedEventArgs e)
        {
            Mediator.NotifyColleagues(MediatorToken.UDPReceived, e);
        }

        private void OnUdpReceived(object evt)
        {
            if (evt is ReceivedEventArgs e)
            {
                IPEndPoint from = e.From;
                //RobotMessage parsed = ParseMessage(e.Bytes);
                Robot sender, receiver;

                if (from.Equals(Robot1.IPEndPoint))
                {
                    sender = Robot1;
                    receiver = Robot2;
                }
                else if (from.Equals(Robot2.IPEndPoint))
                {
                    sender = Robot2;
                    receiver = Robot1;
                }
                else return;

                // handle robot position message
                try
                {
                    if (e.Bytes[0] == 'x')
                    {
                        string[] pos = { "", "", "" };

                        int offset = 1, posIdx = 0;
                        var bytes = e.Bytes[offset..];
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            if ((char)bytes[i] == ',')
                            {
                                posIdx++;
                            }
                            else pos[posIdx] += (char)bytes[i];
                        }

                        float xFactor = 294 / (float)600;
                        float yFactor = 222 / (float)400;

                        sender.Packets.Push(new Packet(DateTime.Now, PacketType.Recv, e.Bytes));
                        sender.Pos = new Position(
                            xFactor * float.Parse(pos[0], CultureInfo.InvariantCulture),
                            yFactor * float.Parse(pos[1], CultureInfo.InvariantCulture),
                            float.Parse(pos[2], CultureInfo.InvariantCulture)
                        );
                        return;
                    }
                }
                catch (Exception er)
                {
                    MessageBox.Show(er.Message);
                    return;
                }

                sender.Packets.Push(new Packet(DateTime.Now, PacketType.Recv, e.Bytes));

                // forward message
                _ = UdpClient.Send(receiver.Id, e.Bytes);
            }
        }

        private void OnNetChanged(object evt)
        {
            HostIP = Networking.GetWiFiIP();
        }

        //struct RecvMessage
        //{
        //    public string Dest { get; set; }
        //    public object[] Args { get; set; }
        //    public string Command { get; set; }
        //    public int Kes { get; set; }
        //    public float X { get; set; }
        //    public float Y { get; set; }
        //    public float Z { get; set; }
        //}

        /// <summary>
        /// format --> from,cmd,...args
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        //private static RobotMessage ParseMessage(byte[] bytes)
        //{
        //    string raw = Encoding.UTF8.GetString(bytes);
        //    string[] tokens = raw.Split(",");

        //    var m = new RobotMessage(
        //        command: tokens[0],
        //        kes: int.Parse(tokens[1]),
        //        x: float.Parse(tokens[2], CultureInfo.InvariantCulture),
        //        y: float.Parse(tokens[3], CultureInfo.InvariantCulture),
        //        z: float.Parse(tokens[4], CultureInfo.InvariantCulture)
        //    );

        //    return m;
        //}
    }
}
