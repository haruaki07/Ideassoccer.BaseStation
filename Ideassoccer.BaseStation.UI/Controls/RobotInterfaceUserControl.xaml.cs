using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace Ideassoccer.BaseStation.UI.Controls
{
    /// <summary>
    /// Interaction logic for RobotInterfaceUserControl.xaml
    /// </summary>
    public partial class RobotInterfaceUserControl : UserControl
    {
        public RobotInterfaceUserControl()
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
