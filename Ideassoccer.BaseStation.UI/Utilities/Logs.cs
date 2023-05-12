using Ideassoccer.BaseStation.UI.ViewModels;

namespace Ideassoccer.BaseStation.UI.Utilities
{
    public class Logs
    {
		public static void Push(string message)
        {
            Mediator.NotifyColleagues(MediatorToken.LogPush, message);
        }
    }
}
