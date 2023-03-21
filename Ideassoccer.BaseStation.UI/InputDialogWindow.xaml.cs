using AakStudio.Shell.UI.Controls;
using System;
using System.Windows;

namespace Ideassoccer.BaseStation.UI
{
    /// <summary>
    /// Interaction logic for InputDialogWindow.xaml
    /// </summary>
    public partial class InputDialogWindow : CustomChromeWindow
    {
        public InputDialogWindow(string label, string defaultValue)
        {
            InitializeComponent();

            lblField.Content = label;
            txtValue.Text = defaultValue;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtValue.SelectAll();
            txtValue.Focus();
        }

        public string Answer
        {
            get { return txtValue.Text; }
        }
    }
}
