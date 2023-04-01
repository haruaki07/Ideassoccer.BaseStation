using Ideassoccer.BaseStation.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Ideassoccer.BaseStation.UI.ViewModels
{
    public class BaseStationViewModel : BaseViewModel
    {
        public ICommand SendMessageCommand { get; set; }
        public ICommand BroadcastMessageCommand { get; set; }

        private RobotUdpClient _udpClient;
        private Dictionary<string, string> _cbItems;
        public Dictionary<string, string> CbItems
        {
            get => _cbItems;
            set => RaisePropertyChanged(ref _cbItems, value);
        }
        private int _selectedDest;
        public int SelectedDest
        {
            get => _selectedDest;
            set => RaisePropertyChanged(ref _selectedDest, value);
        }

        public struct Icons
        {
            public BitmapImage Start => new(new Uri("pack://application:,,,/Ideassoccer.BaseStation.UI;component/Resources/icons/run.png"));
            public BitmapImage Stop => new(new Uri("pack://application:,,,/Ideassoccer.BaseStation.UI;component/Resources/icons/stop.png"));
        }
        public Icons Icon => new Icons();

        public BaseStationViewModel(RobotUdpClient udpClient, Dictionary<string, string> cbItems)
        {
            _udpClient = udpClient;
            _cbItems = cbItems;
            _selectedDest = 0;

            SendMessageCommand = new CommandParam<string>(HandleSendMessageCommand);
            BroadcastMessageCommand = new CommandParam<string>(HandleBroadcastMessageCommand);
        }

        private void HandleSendMessageCommand(string msg)
        {
            _ = _udpClient.Send(_cbItems.ElementAt(SelectedDest).Key, msg);
        }

        private void HandleBroadcastMessageCommand(string msg)
        {
            // 0 means all robot
            _ = _udpClient.Send("0", msg);
        }
    }

    public static class MessagePayload
    {
        public static string Start = "s";
        public static string Stop = "S";
        public static string Ping = "ping";
        public static string RKickOff = "k";
        public static string LKickOff = "K";
        public static string RCornerKick = "c";
        public static string LCornerKick = "C";
        public static string DropBall = "N";
    }
}
