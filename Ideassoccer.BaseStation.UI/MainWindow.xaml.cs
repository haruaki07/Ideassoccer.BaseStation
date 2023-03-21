using AakStudio.Shell.UI.Controls;
using Ideassoccer.BaseStation.UI.Utilities;
using Ideassoccer.BaseStation.UI.ViewModels;
using System.Collections.Generic;
using System.Net;
using System.Windows;

namespace Ideassoccer.BaseStation.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : CustomChromeWindow
    {
        private Udp _udp;

        public int UdpPort { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            UdpPort = 4242;
            _udp = new Udp(new IPEndPoint(IPAddress.Any, UdpPort));
            var mainVm = new MainViewModel(_udp);

            DataContext = mainVm;

            var cbItems = new Dictionary<string, string>
            {
                { "0", "All"},
                {mainVm.Robot1.Id, mainVm.Robot1.Name },
                {mainVm.Robot2.Id, mainVm.Robot2.Name },
            };
            baseStationControl.DataContext = new BaseStationViewModel(mainVm.UdpClient, cbItems);

            robot1Control.DataContext = new RobotViewModel(mainVm.Robot1);
            robot2Control.DataContext = new RobotViewModel(mainVm.Robot2);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _ = _udp.Listen();
        }
    }
}
