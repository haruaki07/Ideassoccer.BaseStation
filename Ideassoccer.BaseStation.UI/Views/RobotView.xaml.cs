using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace Ideassoccer.BaseStation.UI.Views
{
    /// <summary>
    /// Interaction logic for RobotView.xaml
    /// </summary>
    public partial class RobotView : UserControl
    {
        public RobotView()
        {
            InitializeComponent();
        }

        private void btnEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            InputDialogWindow inputDialog = new("IP Address:", txtAddr.Text);
            if (inputDialog.ShowDialog() == true)
            {
                if (!IPEndPoint.TryParse(inputDialog.txtValue.Text, out IPEndPoint? ep))
                {
                    MessageBox.Show("Invalid IP Endpoint!");
                    return;
                }
            }
        }

        private void dgCtxClear_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
