using Ideassoccer.BaseStation.UI.Utilities;
using System.Windows.Input;

namespace Ideassoccer.BaseStation.UI.ViewModels
{
    public class LogViewModel : BaseViewModel
    {
        public ICommand ClearLogsCommand { get; set; }
        public ICommand CloseLogsCommand { get; set; }

        private string _logs;
        public string Logs
        {
            get => _logs;
            set => RaisePropertyChanged(ref _logs, value);
        }

        private System.Action ScrollToBottom;

        public LogViewModel(System.Action scrollAction)
        {
            ClearLogsCommand = new Command(() => Logs = "");
            CloseLogsCommand = new Command(() => Mediator.NotifyColleagues(MediatorToken.LogClose, 0));

            this._logs = "";
            this.ScrollToBottom = scrollAction;


            Mediator.Register(MediatorToken.LogPush, OnLogPush);
        }

        private void OnLogPush(object message)
        {
            if (message is string msg)
            {
                Logs += msg
                    .Trim() + "\n";
                ScrollToBottom();
            }
        }
    }
}
