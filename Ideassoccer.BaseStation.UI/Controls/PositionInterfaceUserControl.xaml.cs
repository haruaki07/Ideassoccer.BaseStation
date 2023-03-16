using Ideassoccer.BaseStation.UI.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ideassoccer.BaseStation.UI.Controls
{
    /// <summary>
    /// Interaction logic for PositionInterfaceUserControl.xaml
    /// </summary>
    public partial class PositionInterfaceUserControl : UserControl
    {
        public PositionInterfaceUserControl()
        {
            InitializeComponent();
            this.DataContext = new PositionViewModel();

            var bg = Application.Current.FindResource("imgField") as BitmapImage;
            canvasRoot.Background = new ImageBrush(bg);
        }
    }
}
