using Ideassoccer.BaseStation.UI.Utilities;
using Ideassoccer.BaseStation.UI.ViewModels;
using System.Windows.Controls;

namespace Ideassoccer.BaseStation.UI.Views
{
    /// <summary>
    /// Interaction logic for RefBoxView.xaml
    /// </summary>
    public partial class RefBoxView : UserControl
    {
        public RefBoxView()
        {
            InitializeComponent();
            Mediator.Register(MediatorToken.RefBoxReceived, OnRefBoxReceived);
        }

        public void OnRefBoxReceived(object e)
        {
            this.Dispatcher.Invoke(() =>
            {
                logsTextBox.ScrollToEnd();
            });
        }
    }
}
