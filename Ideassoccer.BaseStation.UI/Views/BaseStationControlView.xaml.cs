using Ideassoccer.BaseStation.UI.ViewModels;
using System.Diagnostics;
using System.Windows.Controls;

namespace Ideassoccer.BaseStation.UI.Views
{
    /// <summary>
    /// Interaction logic for BaseStationControlView.xaml
    /// </summary>
    public partial class BaseStationControlView : UserControl
    {
        public BaseStationViewModel ViewModel
        {
            get => (BaseStationViewModel)DataContext;
        }

        public BaseStationControlView()
        {
            InitializeComponent();
        }
    }
}
