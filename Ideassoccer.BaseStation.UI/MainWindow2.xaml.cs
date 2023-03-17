using Ideassoccer.BaseStation.UI.Utilities;
using Ideassoccer.BaseStation.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Interop;

namespace Ideassoccer.BaseStation.UI
{
    /// <summary>
    /// Interaction logic for MainWindow2.xaml
    /// </summary>
    public partial class MainWindow2 : Window
    {
        private Udp _udp;

        public int UdpPort { get; set; }

        public MainWindow2()
        {
            InitializeComponent();

            UdpPort = 4242;
            _udp = new Udp(new IPEndPoint(IPAddress.Any, UdpPort));
            var mainVm = new MainViewModel();
            var robotUdpClient = new RobotUdpClient(_udp, new Dictionary<string, Robot>
            {
                { mainVm.Robot1.Id, mainVm.Robot1},
                { mainVm.Robot2.Id, mainVm.Robot2},
            });

            DataContext = mainVm;

            var cbItems = new Dictionary<string, string>
            {
                { "0", "All"},
                {mainVm.Robot1.Id, mainVm.Robot1.Name },
                {mainVm.Robot2.Id, mainVm.Robot2.Name },
            };
            baseStationControl.DataContext = new BaseStationViewModel(robotUdpClient, cbItems);
        }

        /// <summary>
        ///  Disable title bar double-click and prevent move window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
        }

        private const int WM_NCLBUTTONDBLCLK = 0x00A3; // title bar double click
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MOVE = 0xF010;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_SYSCOMMAND:
                    int command = wParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                        handled = true;
                    break;
            }

            if (msg == WM_NCLBUTTONDBLCLK)
            {
                handled = true;  //prevent double click from maximizing the window.
            }

            return IntPtr.Zero;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _ = _udp.Listen();
        }
    }
}
