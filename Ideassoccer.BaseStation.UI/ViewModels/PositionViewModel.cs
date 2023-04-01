using Ideassoccer.BaseStation.UI.Models;
using Ideassoccer.BaseStation.UI.Utilities;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ideassoccer.BaseStation.UI.ViewModels
{
    public class PositionViewModel : BaseViewModel
    {
        private ImageBrush _canvasBackground;

        public ImageBrush CanvasBackground
        {
            get => _canvasBackground;
            set => RaisePropertyChanged(ref _canvasBackground, value);
        }

        private Robot _robot1;
        public Robot Robot1
        {
            get => _robot1;
            set => RaisePropertyChanged(ref _robot1, value);
        }

        private Robot _robot2;
        public Robot Robot2
        {
            get => _robot2;
            set => RaisePropertyChanged(ref _robot2, value);
        }

        public PositionViewModel(Robot robot1, Robot robot2)
        {
            Mediator.Register(MediatorToken.Robot1Moved, OnRobot1Moved);
            Mediator.Register(MediatorToken.Robot2Moved, OnRobot2Moved);

            var bg = new BitmapImage(new Uri("pack://application:,,,/Ideassoccer.BaseStation.UI;component/Resources/field.png"));
            _canvasBackground = new ImageBrush(bg);

            _robot1 = robot1;
            _robot2 = robot2;
        }

        void OnRobot1Moved(object evt)
        {
            if (evt is Position e)
                Robot1.Pos = e;
        }

        void OnRobot2Moved(object evt)
        {
            if (evt is Position e)
                Robot2.Pos = e;
        }
    }
}
