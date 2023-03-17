using Ideassoccer.BaseStation.UI.ViewModels;
using System.Diagnostics;
using System.Windows.Controls;

namespace Ideassoccer.BaseStation.UI.Controls
{
    /// <summary>
    /// Interaction logic for BaseStationInterfaceUserControl.xaml
    /// </summary>
    public partial class BaseStationInterfaceUserControl : UserControl
    {
        public BaseStationViewModel ViewModel
        {
            get => (BaseStationViewModel)DataContext;
        }

        public BaseStationInterfaceUserControl()
        {
            InitializeComponent();
        }
    }
}
