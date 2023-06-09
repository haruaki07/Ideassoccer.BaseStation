using Ideassoccer.BaseStation.UI.Models;
using Ideassoccer.BaseStation.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
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
        public ICommand OpenLogsCommand { get; set; }

        public string PageTitle { get; private set; }
        private Udp _udp;
        public RobotUdpClient UdpClient;

        private int _udpPort = 4242;
        public int UdpPort
        {
            get => _udpPort;
            set => RaisePropertyChanged(ref _udpPort, value);
        }

        private Robot _robotGk;
        public Robot RobotGk
        {
            get => _robotGk;
            set => RaisePropertyChanged(ref _robotGk, value);
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

        private RobotViewModel _robotGkvm;
        public RobotViewModel RobotGkVM
        {
            get => _robotGkvm;
            set => RaisePropertyChanged(ref _robotGkvm, value);
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

        private RefBoxViewModel _refboxVM;
        public RefBoxViewModel RefBoxVM
        {
            get => _refboxVM;
            set => RaisePropertyChanged(ref _refboxVM, value);
        }

        private string? _hostIP;
        public string? HostIP { get => _hostIP; set => RaisePropertyChanged(ref _hostIP, value); }

        private bool _showLogs;
        public bool ShowLogs
        {
            get => _showLogs;
            set => RaisePropertyChanged(ref _showLogs, value);
        }

        private State _state;
        public State State
        {
            get => _state;
            set => RaisePropertyChanged(ref _state, value);
        }

        private PlayMode _playMode;
        public PlayMode PlayMode
        {
            get => _playMode;
            set => RaisePropertyChanged(ref _playMode, value);
        }
        #endregion

        #region ctor
        public MainViewModel()
        {
            PageTitle = "Ideassoccer BaseStation " + GetVersionInfoString();

            Mediator.Register(MediatorToken.UDPReceived, OnUdpReceived);
            Mediator.Register(MediatorToken.NetworkInterfaceChanged, OnNetChanged);
            Mediator.Register(MediatorToken.LogClose, (object e) => ShowLogs = false);

            _showLogs = false;
            OpenLogsCommand = new Command(() => ShowLogs = true);

            _state = State.Stop;
            _playMode = PlayMode.RKickOff;

            _udp = new Udp(new IPEndPoint(IPAddress.Any, UdpPort));
            _udp.Received += _udp_Received;

            _robotGk = new Robot(
                Properties.Settings.Default.RobotGKId,
                Properties.Settings.Default.RobotGKName,
                new IPEndPoint(
                    IPAddress.Parse(Properties.Settings.Default.RobotGKEndpointAddress),
                    Properties.Settings.Default.RobotGKEndpointPort
                ),
                null
            );
            _robot1 = new Robot(
                Properties.Settings.Default.Robot1Id,
                Properties.Settings.Default.Robot1Name,
                new IPEndPoint(
                    IPAddress.Parse(Properties.Settings.Default.Robot1EndpointAddress),
                    Properties.Settings.Default.Robot1EndpointPort
                ),
                null
            );
            _robot2 = new Robot(
                Properties.Settings.Default.Robot2Id,
                Properties.Settings.Default.Robot2Name,
                new IPEndPoint(
                    IPAddress.Parse(Properties.Settings.Default.Robot2EndpointAddress),
                    Properties.Settings.Default.Robot2EndpointPort
                ),
                null
            );

            UdpClient = new RobotUdpClient(_udp, new Robots
            {
                { RobotGk.Id, RobotGk },
                { Robot1.Id, Robot1 },
                { Robot2.Id, Robot2 },
            });

            _robotGkvm = new RobotViewModel(_robotGk, Properties.Settings.Default.RobotGKUdpPort);
            _robot1vm = new RobotViewModel(_robot1, Properties.Settings.Default.Robot1UdpPort);
            _robot2vm = new RobotViewModel(_robot2, Properties.Settings.Default.Robot2UdpPort);

            _posvm = new PositionViewModel(Robot1, Robot2);

            _cbItems = new Dictionary<string, string>
            {
                { "0", "All"},
                {_robotGk.Id, _robotGk.Name },
                {_robot1.Id, _robot1.Name },
                {_robot2.Id, _robot2.Name },
            };
            _bstavm = new BaseStationViewModel(
                UdpClient,
                CbItems,
                _state,
                new Action<State>((newState) => State = newState),
                _playMode,
                new Action<PlayMode>((newMode) => PlayMode = newMode)
            );

            IPEndPoint refboxEp;
            if (!IPEndPoint.TryParse(Properties.Settings.Default.RefBoxEndPoint, out refboxEp!))
                refboxEp = new IPEndPoint(IPAddress.Loopback, 9000);

            _refboxVM = new RefBoxViewModel(refboxEp);

            ListenUdpCommand = new Command(() => _ = _udp.Listen());
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

                        // based on field image size (222px x 294px) and real size (400cm x 600cm)
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

                    byte[] msg = { e.Bytes[0], (byte)',', (byte)State.Value[0], (byte)';', (byte)PlayMode.Value[0] };

                    sender.Packets.Push(new Packet(DateTime.Now, PacketType.Recv, e.Bytes));

                    // forward message
                    _ = UdpClient.Send(receiver.Id, msg);
                }
                catch (Exception er)
                {
                    Logs.Push(er.ToString());
                    return;
                }
            }
        }

        private void OnNetChanged(object evt)
        {
            HostIP = Networking.GetWiFiIP();
        }

        private string GetVersionInfoString()
        {
            return "v" + Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? "";
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
