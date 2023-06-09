using Ideassoccer.BaseStation.UI.Utilities;
using SuperSimpleTcp;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Ideassoccer.BaseStation.UI.ViewModels
{
    public class RefBoxViewModel : BaseViewModel
    {
        public ICommand ConnectCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand EditEndpointCommand { get; set; }
        public ICommand ClearLogsCommand { get; set; }

        private SimpleTcpClient _client;

        private IPEndPoint _ep;
        public IPEndPoint Ep
        {
            get => _ep;
            set => RaisePropertyChanged(ref _ep, value);
        }

        private bool _loading;
        public bool Loading
        {
            get => _loading;
            set => RaisePropertyChanged(ref _loading, value);
        }

        private bool _connected;
        public bool Connected
        {
            get => _connected;
            set => RaisePropertyChanged(ref _connected, value);
        }

        private string _refBoxLogs;
        public string RefBoxLogs
        {
            get => _refBoxLogs;
            set => RaisePropertyChanged(ref _refBoxLogs, value);
        }

        public RefBoxViewModel(IPEndPoint ep)
        {
            _ep = ep;
            InitTcpClient(out _client, ep);
            _connected = false;
            _refBoxLogs = "";

            ConnectCommand = new Command(HandleConnectCommand);
            DisconnectCommand = new Command(HandleDisconnectCommand);
            EditEndpointCommand = new Command(HandleEditEndpointCommand);
            ClearLogsCommand = new Command(() => RefBoxLogs = "");
        }

        private async void HandleDisconnectCommand()
        {
            try
            {
                Loading = true;
                RefBoxLogPush("> Trying to disconnect from RefBox...");
                await _client.DisconnectAsync();
            }
            catch (Exception e)
            {
                Logs.Push("Error disconnecting RefBox: " + e.Message);
            }
            finally
            {
                Loading = false;
            }
        }

        private async void HandleConnectCommand()
        {
            try
            {
                Loading = true;
                RefBoxLogPush("> Trying to connect RefBox...");
                await Task.Run(() => _client.Connect());
            }
            catch (Exception e)
            {
                RefBoxLogPush("> Failed to connect RefBox!");
                Logs.Push("Error connecting RefBox: " + e.Message);
            }
            finally
            {
                Loading = false;
            }
        }

        private void HandleEditEndpointCommand()
        {
            var inputDialog = new InputDialogWindow("IP Address:", Ep.ToString());
            if (inputDialog.ShowDialog() == true)
            {
                IPEndPoint ep;
                if (!IPEndPoint.TryParse(inputDialog.txtValue.Text, out ep!))
                {
                    MessageBox.Show("Invalid IP Endpoint!");
                    return;
                }

                Ep = ep;
                InitTcpClient(out _client, Ep);

                try
                {
                    Properties.Settings.Default.RefBoxEndPoint = Ep.ToString();
                    Properties.Settings.Default.Save();
                }
                catch { }
            }
        }

        private void _client_Received(object? sender, DataReceivedEventArgs e)
        {
#if !DEBUG
            if (!e.From.Equals(Ep))
                return;
#endif

            RefBoxLogPush(Encoding.UTF8.GetString(e.Data));
        }

        private void _client_Disconnected(object? sender, ConnectionEventArgs e)
        {
            Connected = false;
            RefBoxLogPush("> Disconnected from RefBox!");
        }

        private void _client_Connected(object? sender, ConnectionEventArgs e)
        {
            Connected = true;
            RefBoxLogPush("> Connected to RefBox!");
        }

        private void InitTcpClient(out SimpleTcpClient c, IPEndPoint ep, int timeoutMs = 3000)
        {
            c = new SimpleTcpClient(ep);
            c.Settings.ConnectTimeoutMs = timeoutMs;
            c.Events.Connected += _client_Connected;
            c.Events.Disconnected += _client_Disconnected;
            c.Events.DataReceived += _client_Received;
        }

        private void RefBoxLogPush(string msg)
        {
            RefBoxLogs += DateTime.Now.ToString("HH:mm:ss") + " " +  msg + Environment.NewLine;
            Mediator.NotifyColleagues(MediatorToken.RefBoxReceived, 0);
        }
    }
}
