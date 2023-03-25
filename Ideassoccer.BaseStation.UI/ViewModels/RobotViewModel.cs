using Ideassoccer.BaseStation.UI.Models;
using Ideassoccer.BaseStation.UI.Utilities;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Input;

namespace Ideassoccer.BaseStation.UI.ViewModels
{
    public class RobotViewModel : BaseViewModel
    {
        public ICommand ClearPacketsCommand { get; set; }
        public ICommand EditEndpointCommand { get; set; }
        public ICommand EditUdpPortCommand { get; set; }
        public ICommand ListenUdpCommand { get; set; }

        private Robot _robot;
        public Robot Robot
        {
            get => _robot;
            set => RaisePropertyChanged(ref _robot, value);
        }

        private int _udpPort;
        public int UdpPort
        {
            get => _udpPort;
            set => RaisePropertyChanged(ref _udpPort, value);
        }

        private Udp _udp;

        public RobotViewModel(Robot robot, int udpPort)
        {
            this._robot = robot;
            _udpPort = udpPort;
            _udp = new Udp(new IPEndPoint(IPAddress.Any, _udpPort));
            _udp.Received += _udp_Received;

            this.ClearPacketsCommand = new Command(() => Robot.Packets.Clear());
            this.EditEndpointCommand = new Command(HandleEditEndpointCommand);
            this.EditUdpPortCommand = new Command(HandleEditUdpPortCommand);
            this.ListenUdpCommand = new Command(() => { _ = _udp.Listen(); });
        }

        private void _udp_Received(object? sender, ReceivedEventArgs e)
        {
            if (e.From.Equals(Robot.IPEndPoint))
                Mediator.NotifyColleagues(MediatorToken.UDPReceived, e);
        }

        private void HandleEditEndpointCommand()
        {
            var inputDialog = new InputDialogWindow("IP Address:", Robot.IPEndPoint.ToString());
            if (inputDialog.ShowDialog() == true)
            {
                IPEndPoint ep;
                if (!IPEndPoint.TryParse(inputDialog.txtValue.Text, out ep!))
                {
                    MessageBox.Show("Invalid IP Endpoint!");
                    return;
                }

                Robot.IPEndPoint = ep;
            }
        }

        private void HandleEditUdpPortCommand()
        {
            var inputDialog = new InputDialogWindow("Port:", UdpPort.ToString());
            if (inputDialog.ShowDialog() == true)
            {
                int port;
                // port must be a number and must be in range 1024 <= p <= 65535
                if (!int.TryParse(inputDialog.txtValue.Text, out port!) || port > 65535 || port < 1024)
                {
                    MessageBox.Show("Invalid port number!");
                    return;
                }

                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] udpConnInfo = ipGlobalProperties.GetActiveUdpListeners();

                foreach (var ep in udpConnInfo)
                {
                    if (ep.Port == port)
                    {
                        MessageBox.Show("Port had already been used! Try another one.");
                        return;
                    }
                }

                UdpPort = port;
            }
        }
    }
}
