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

        public PositionViewModel()
        {
            var bg = new BitmapImage(new Uri("pack://application:,,,/Ideassoccer.BaseStation.UI;component/Resources/field.png"));
            _canvasBackground = new ImageBrush(bg);
        }
    }
}
