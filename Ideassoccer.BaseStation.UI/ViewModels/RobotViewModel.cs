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

        private IPEndPoint _udpEndPoint;
        public IPEndPoint UdpEndPoint
        {
            get => _udpEndPoint;
            set => RaisePropertyChanged(ref _udpEndPoint, value);
        }

        private Udp _udp;

        public RobotViewModel(Robot robot, int udpPort)
        {
            this._robot = robot;
            _udpEndPoint = new IPEndPoint(IPAddress.Any, udpPort);
            _udp = new Udp(_udpEndPoint);
            _udp.Received += _udp_Received;

            this.ClearPacketsCommand = new Command(() => Robot.Packets.Clear());
            this.EditEndpointCommand = new Command(HandleEditEndpointCommand);
            this.EditUdpPortCommand = new Command(HandleEditUdpPortCommand);
            this.ListenUdpCommand = new Command(() => { _ = _udp.Listen(_udpEndPoint); });
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
            var inputDialog = new InputDialogWindow("Port:", _udpEndPoint.Port.ToString());
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

                UdpEndPoint = new IPEndPoint(_udpEndPoint.Address, port);
                _udp.StopListening();

                ListenUdpCommand.Execute(null);
            }
        }
    }
}
