using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Ideassoccer.BaseStation.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Udp _udp;
        private bool _udpStarted = false;

        public Dictionary<string, string> CbItems { get; set; }
        public Robot Robot1 { get; set; }
        public Robot Robot2 { get; set; }
        public bool IsUdpStarted
        {
            get
            {
                return _udpStarted;
            }
            set
            {
                _udpStarted = value;
                NotifyPropertyChanged("IsUdpStarted");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Robot1 = new Robot("1", "Robot 1", new IPEndPoint(IPAddress.Parse("192.168.8.150"), 4242));
            Robot2 = new Robot("2", "Robot 2", new IPEndPoint(IPAddress.Parse("192.168.8.151"), 4242));

            CbItems = new Dictionary<string, string>
            {
                { "0", "All" },
                { Robot1.Id, "Robot 1" },
                { Robot2.Id, "Robot 2" }
            };

            _udp = new Udp(new IPEndPoint(IPAddress.Any, 4242));
            _udp.Started += _udp_Started;
            _udp.Stopped += _udp_Stopped;
            _udp.Received += _udp_Received;
        }

        private void _udp_Started(object? sender, EventArgs e)
        {
            IsUdpStarted = true;
        }

        private void _udp_Stopped(object? sender, EventArgs e)
        {
            IsUdpStarted = false;
        }

        private void _udp_Received(object? sender, ReceivedEventArgs e)
        {
            IPEndPoint from = e.From;

            // If packet is from robot1 then forward to robot2
            if (from.Address.ToString() == Robot1.IPEndPoint.Address.ToString())
            {
                Robot1.Packets.Push(new Packet(DateTime.Now, PacketType.Recv, e.Bytes));
                _ = SendPacket(Robot2.Id, e.Bytes);
            }
            // If packet is from robot2 then forward to robot1
            else if (from.Address.ToString() == Robot2.IPEndPoint.Address.ToString())
            {
                Robot2.Packets.Push(new Packet(DateTime.Now, PacketType.Recv, e.Bytes));
                _ = SendPacket(Robot1.Id, e.Bytes);
            }
        }


        private async Task SendPacket(string dest, byte[] dgram)
        {
            if (dgram.Length <= 0) return;

            if (dest == "0")
            {
                foreach (var r in new Robot[] { Robot1, Robot2 })
                {
                    await _udp.Send(r.IPEndPoint, dgram);
                    r.Packets.Push(new Packet(DateTime.Now, PacketType.Send, dgram));
                }
            } else if (dest == Robot1.Id)
            {
                await _udp.Send(Robot1.IPEndPoint, dgram);
                Robot1.Packets.Push(new Packet(DateTime.Now, PacketType.Send, dgram));
            }
            else if (dest == Robot2.Id)
            {
                await _udp.Send(Robot2.IPEndPoint, dgram);
                Robot2.Packets.Push(new Packet(DateTime.Now, PacketType.Send, dgram));
            }
        }

        private async Task SendPacket(string dest, string msg)
        {
            await SendPacket(dest, Encoding.UTF8.GetBytes(msg.Trim()));
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var msg = textBoxSend.Text;
            var dest = (string)cbSendDest.SelectedValue;

            _ = SendPacket(dest, msg);

            textBoxSend.Focus();
        }

        private void textBoxSend_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var msg = textBoxSend.Text;
                var dest = (string)cbSendDest.SelectedValue;

                _ = SendPacket(dest, msg);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            _ = _udp.Listen();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            _udp.StopListening();
        }

        private void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

}
