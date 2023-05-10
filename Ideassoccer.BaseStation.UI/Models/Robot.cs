using Ideassoccer.BaseStation.UI.Utilities;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;

namespace Ideassoccer.BaseStation.UI.Models
{
    public class Robot : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        private string _name;
        public string Name
        {
            get => _name;
            set => NotifyPropertyChanged(ref _name, value);
        }

        private IPEndPoint _ipEndpoint;
        public IPEndPoint IPEndPoint
        {
            get => _ipEndpoint;
            set => NotifyPropertyChanged(ref _ipEndpoint, value);
        }

        private readonly string _id;
        public string Id { get => _id; }

        public ObservableStack<Packet> Packets { get; set; }

        private Position? _pos;
        public Position? Pos
        {
            get => _pos;
            set => NotifyPropertyChanged(ref _pos, value);
        }

        public Robot(string id, string name, IPEndPoint endpoint, Position? pos)
        {
            this._id = id;
            this._name = name;
            _ipEndpoint = endpoint;
            Packets = new ObservableStack<Packet>();
            _pos = pos;
        }

        public string GetIPAddress()
        {
            return IPEndPoint.Address.ToString();
        }

        protected void NotifyPropertyChanged<T>(ref T property, T newValue, [CallerMemberName] string propertyName = "")
        {
            property = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Position : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private float? _x;
        public float? X
        {
            get => _x;
            set => NotifyPropertyChanged(ref _x, value);
        }

        private float? _y;
        public float? Y
        {
            get => _y;
            set => NotifyPropertyChanged(ref _y, value);
        }

        private float? _z;
        public float? Z
        {
            get => _z;
            set => NotifyPropertyChanged(ref _z, value);
        }

        public Position() { }
        public Position(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        protected void NotifyPropertyChanged<T>(ref T property, T newValue, [CallerMemberName] string propertyName = "")
        {
            property = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
