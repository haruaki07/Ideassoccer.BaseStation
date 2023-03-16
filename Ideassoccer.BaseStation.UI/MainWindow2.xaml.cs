using System;
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
        public Robot Robot1 { get; set; }

        public MainWindow2()
        {
            InitializeComponent();
            DataContext = this;

            Robot1 = new Robot("1", "Robot 1", new IPEndPoint(IPAddress.Parse("192.168.8.150"), 4242));
        }

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
    }
}
