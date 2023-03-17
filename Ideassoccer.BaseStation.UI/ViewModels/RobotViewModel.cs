using Ideassoccer.BaseStation.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Ideassoccer.BaseStation.UI.ViewModels
{
    public class RobotViewModel : BaseViewModel
    {
        public ICommand ClearPacketsCommand { get; set; }
        public ICommand EditEndpointCommand { get; set; }

        private Robot _robot;

        public Robot Robot
        {
            get => _robot;
            set => RaisePropertyChanged(ref _robot, value);
        }

        public RobotViewModel(Robot robot)
        {
            this._robot = robot;
            this.ClearPacketsCommand = new Command(() =>
            {
                this.Robot.Packets.Clear();
            });
            this.EditEndpointCommand = new Command(HandleEditEndpointCommand);
        }

        private void HandleEditEndpointCommand()
        {
            InputDialogWindow inputDialog = new InputDialogWindow("IP Address:", Robot.IPEndPoint.ToString());
            if (inputDialog.ShowDialog() == true)
            {
                IPEndPoint ep;
                if (!IPEndPoint.TryParse(inputDialog.txtValue.Text, out ep!))
                {
                    MessageBox.Show("Invalid IP Endpoint!");
                    return;
                }

                Robot.IPEndPoint = ep;
            }
        }
    }
}
