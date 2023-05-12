using Ideassoccer.BaseStation.UI.ViewModels;
using System;
using System.Windows.Controls;

namespace Ideassoccer.BaseStation.UI.Views
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        public LogView()
        {
            InitializeComponent();

            Action scrollAction = new(() =>
            {
                if (logScrollViewer.Template.FindName("PART_ContentHost", logScrollViewer) is ScrollViewer scrollViewer)
                    scrollViewer.ScrollToBottom();
            });
            this.DataContext = new LogViewModel(scrollAction);
        }
    }
}
