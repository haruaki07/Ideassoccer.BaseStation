using Ideassoccer.BaseStation.UI.Enums;
using Ideassoccer.BaseStation.UI.Models;
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
        public ICommand BroadcastStateCommand { get; set; }
        public ICommand BroadcastPlayModeCommand { get; set; }

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

        private State _state;
        public State State
        {
            get => _state;
            set => RaisePropertyChanged(ref _state, value);
        }
        private Action<State> _onStateChange;

        private PlayMode _playMode;
        public PlayMode PlayMode
        {
            get => _playMode;
            set => RaisePropertyChanged(ref _playMode, value);
        }
        private Action<PlayMode> _onPlayModeChange;

        public struct Icons
        {
            public BitmapImage Start =>
                new(
                    new Uri(
                        "pack://application:,,,/Ideassoccer.BaseStation.UI;component/Resources/icons/run.png"
                    )
                );
            public BitmapImage Stop =>
                new(
                    new Uri(
                        "pack://application:,,,/Ideassoccer.BaseStation.UI;component/Resources/icons/stop.png"
                    )
                );
        }

        public Icons Icon => new Icons();

        public BaseStationViewModel(
            RobotUdpClient udpClient,
            Dictionary<string, string> cbItems,
            State state,
            Action<State> onStateChange,
            PlayMode playMode,
            Action<PlayMode> onPlayModeChange
        )
        {
            _udpClient = udpClient;
            _cbItems = cbItems;
            _selectedDest = 0;
            _state = state;
            _onStateChange = onStateChange;
            _playMode = playMode;
            _onPlayModeChange = onPlayModeChange;

            SendMessageCommand = new CommandParam<string>(HandleSendMessageCommand);
            BroadcastStateCommand = new CommandParam<string>(HandleBroadcastStateCommand);
            BroadcastPlayModeCommand = new CommandParam<string>(HandleBroadcastPlayModeCommand);
        }

        private void HandleSendMessageCommand(string msg)
        {
            //_ = _udpClient.Send(_cbItems.ElementAt(SelectedDest).Key, "b", msg);
            if (msg.Length > 0)
                _ = _udpClient.Send(_cbItems.ElementAt(SelectedDest).Key, "0," + msg);
        }

        private void HandleBroadcastPlayModeCommand(string mode)
        {
            // 0 means all robot
            // message == isKick,state
            _ = _udpClient.Send("0", "0," + State.Value + ";" + mode);

            if (mode != MessagePayload.Ping)
            {
                PlayMode = new PlayMode(mode); // ye
                _onPlayModeChange(PlayMode);
            }
        }

        private void HandleBroadcastStateCommand(string state)
        {
            // 0 means all robot
            // message == isKick,state
            _ = _udpClient.Send("0", "0," + state + ";" + PlayMode.Value);

            if (state != MessagePayload.Ping)
            {
                State = new State(state); // ye
                _onStateChange(State);
            }
        }
    }

    public static class MessagePayload
    {
        public static string Start = State.Start.Value;
        public static string Stop = State.Stop.Value;
        public static string Retry = State.Retry.Value;
        public static string Ping = "ping";
        public static string RKickOff = PlayMode.RKickOff.Value;
        public static string LKickOff = PlayMode.LKickOff.Value;
        public static string RCornerKick = PlayMode.RCorner.Value;
        public static string LCornerKick = PlayMode.LCorner.Value;
        public static string DropBall = PlayMode.DropBall.Value;
    }
}
