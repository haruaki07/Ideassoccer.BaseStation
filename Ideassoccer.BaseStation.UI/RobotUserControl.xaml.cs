using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
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

        private void btnEdit_Click(object sender, RoutedEventArgs e)
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

                GetDataContext().IPEndPoint = ep;
            }
        }

        private void dgCtxClear_Click(object sender, RoutedEventArgs e)
        {
            GetDataContext().Packets.Clear();
        }

        private void btnPing_Click(object sender, RoutedEventArgs e)
        {
            // todo
        }

        private Robot GetDataContext()
        {
            return ((Robot)DataContext);
        }
    }
}
