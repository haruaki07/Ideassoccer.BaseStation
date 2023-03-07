using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace Ideassoccer.BaseStation.UI
{
    /// <summary>
    /// Interaction logic for RobotUserControl.xaml
    /// </summary>
    public partial class RobotUserControl : UserControl
    {
        public RobotUserControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InputDialogWindow inputDialog = new InputDialogWindow("IP Address:", txtAddr.Text);
            if (inputDialog.ShowDialog() == true)
            {
                IPEndPoint? ep;
                if (!IPEndPoint.TryParse(inputDialog.txtValue.Text, out ep))
                {
                    MessageBox.Show("Invalid IP Endpoint!");
                    return;
                }

                ((Robot)DataContext).IPEndPoint = ep;
            }
        }

        private void dgCtxClear_Click(object sender, RoutedEventArgs e)
        {
            ((Robot)DataContext).Packets.Clear();
        }
    }
}
