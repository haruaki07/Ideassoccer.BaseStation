using Ideassoccer.BaseStation.UI.ViewModels;
using System.Windows.Controls;

namespace Ideassoccer.BaseStation.UI
{
    /// <summary>
    /// Interaction logic for PositionUserControl.xaml
    /// </summary>
    public partial class PositionUserControl : UserControl
    {
        public PositionUserControl()
        {
            InitializeComponent();
            this.DataContext = new PositionViewModel();
        }
    }
}
