using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Ideassoccer.BaseStation.UI.Utilities
{
    public class Udp
    {
        private UdpClient _socket;
        private IPEndPoint _ipEndPoint;
        public event EventHandler? Started;
        public event EventHandler? Stopped;
        public event EventHandler<ReceivedEventArgs>? Received;
        public event EventHandler? Sent;

        public Udp(IPEndPoint endpoint)
        {
            _ipEndPoint = endpoint;
            _socket = new UdpClient();
        }

        public async Task Listen()
        {
            await Listen(null);
        }

        public async Task Listen(IPEndPoint? ep)
        {
            try
            {
                _socket = new UdpClient(ep ?? _ipEndPoint);

                Started?.Invoke(this, EventArgs.Empty);

                await Receive();
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException) return;

                Logs.Push("Error listen udp: " + e.Message);
                StopListening();
            }
        }

        private async Task Receive()
        {
            var useStopSource = GetCancellationToken();
            var cancelToken = useStopSource.Token;
            Stopped += stopListener;

            do
            {
                if (cancelToken.IsCancellationRequested) break;

                try
                {
                    var res = await _socket.ReceiveAsync(cancelToken);

                    Received?.Invoke(this, new ReceivedEventArgs(res.Buffer, res.RemoteEndPoint));
                }
                finally
                {
                    Stopped -= stopListener;

                    // Cancel token is not reusable
                    useStopSource = GetCancellationToken();
                    cancelToken = useStopSource.Token;
                    Stopped += stopListener;
                }
            } while (true);

            void stopListener(object? sender, EventArgs e)
            {
                useStopSource?.Cancel();
            };
        }

        public void StopListening()
        {
            Stopped?.Invoke(this, EventArgs.Empty);

            _socket?.Client.Dispose();
            _socket?.Dispose();
        }

        public async Task Send(IPEndPoint dest, byte[] dgram)
        {
            using UdpClient socket = new();

            _ = await socket.SendAsync(dgram, dest);

            Sent?.Invoke(this, EventArgs.Empty);

            socket.Client?.Close();
            socket.Dispose();
        }

        private CancellationTokenSource GetCancellationToken()
        {
            var cancelSource = new CancellationTokenSource();

            return cancelSource;
        }
    }
    public class ReceivedEventArgs : EventArgs
    {
        public byte[] Bytes { get; }
        public IPEndPoint From { get; }

        public ReceivedEventArgs(byte[] bytes, IPEndPoint from)
        {
            this.Bytes = bytes;
            this.From = from;
        }
    }
}
