using Ideassoccer.BaseStation.UI.Utilities;
using System.ComponentModel;
using System.Net;

namespace Ideassoccer.BaseStation.UI.Models
{
    public class Robot : INotifyPropertyChanged
    {
        private string name;
        private IPEndPoint ipEndPoint;
        private readonly string id;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public IPEndPoint IPEndPoint
        {
            get
            {
                return ipEndPoint;
            }
            set
            {
                ipEndPoint = value;
                NotifyPropertyChanged("IPEndPoint");
            }
        }

        public string Id
        {
            get
            {
                return id;
            }
        }

        public ObservableStack<Packet> Packets { get; set; }

        public Robot(string id, string name, IPEndPoint endpoint)
        {
            this.id = id;
            this.name = name;
            ipEndPoint = endpoint;
            Packets = new ObservableStack<Packet>();
        }

        public string GetIPAddress()
        {
            return IPEndPoint.Address.ToString();
        }

        private void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
