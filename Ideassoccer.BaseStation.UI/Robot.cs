using System.ComponentModel;
using System.Net;

namespace Ideassoccer.BaseStation.UI
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
                return this.name;
            }
            set
            {
                this.name = value;
                this.NotifyPropertyChanged("Name");
            }
        }
        public IPEndPoint IPEndPoint { 
            get
            {
                return this.ipEndPoint;
            }
            set
            {
                this.ipEndPoint = value;
                this.NotifyPropertyChanged("IPEndPoint");
            }
        }

        public string Id
        {
            get
            {
                return this.id;
            }
        }

        public ObservableStack<Packet> Packets { get; set; }

        public Robot(string id, string name, IPEndPoint endpoint)
        {
            this.id = id;
            this.name = name;
            this.ipEndPoint = endpoint;
            this.Packets = new ObservableStack<Packet>();
        }

        public string GetIPAddress()
        {
            return this.IPEndPoint.Address.ToString();
        }

        private void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
