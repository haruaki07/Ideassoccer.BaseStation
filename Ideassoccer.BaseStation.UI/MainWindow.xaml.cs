using AakStudio.Shell.UI.Controls;
using Ideassoccer.BaseStation.UI.ViewModels;

namespace Ideassoccer.BaseStation.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : CustomChromeWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
