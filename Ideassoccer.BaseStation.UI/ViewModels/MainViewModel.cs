using Ideassoccer.BaseStation.UI.Utilities;
using System.Net;

namespace Ideassoccer.BaseStation.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private Robot _robot1;
        private Robot _robot2;

        public Robot Robot1 {
            get => _robot1;
            set => RaisePropertyChanged(ref _robot1, value);
        }
        public Robot Robot2 { 
            get => _robot2;
            set => RaisePropertyChanged(ref _robot2, value);
        }

        public MainViewModel()
        {
            _robot1 = new Robot("1", "Robot 1", new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4241));
            _robot2 = new Robot("2", "Robot 2", new IPEndPoint(IPAddress.Parse("192.168.8.151"), 4242));
        }
    }
}
