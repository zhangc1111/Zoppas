namespace Zoppas.Model
{
    using System;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    internal class SocketClient
    {
        /// <summary>
        /// When received data, dispatch
        /// </summary>
        internal event Action<byte[]> DataRecv;

        private readonly string _URL;
        private readonly int _PORT;

        private Socket _Connection;
        private bool _IsConnected;
        public bool IsConnected
        {
            get { return _IsConnected; }
        }

        private SocketClient()
        {
            _Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public SocketClient(string url, int port) : this()
        {
            _URL = url;
            _PORT = port;
        }

        public void Connect()
        {
            try
            {
                _Connection.Connect(_URL, _PORT);
                _IsConnected = _Connection.Connected;
                new Task(() => { Recv(); }).Start();
            }
            catch (Exception ex)
            {
                if (_URL == "127.0.0.1" && _PORT == 6000)
                {
                    Logger.Fatal("Vitex disconnected.");
                }
                if (_URL == "192.168.0.1" && _PORT == 2000)
                {
                    Logger.Fatal("Plc disconnected.");
                }
            }
            finally
            {
                // 退出
            }
        }

        public void Send(byte[] buffer)
        {
            try
            {
                _Connection.Send(buffer);
            }
            catch (Exception)
            {

            }
        }

        private void Recv()
        {
            while (true)
            {
                Thread.Sleep(1);

                byte[] source = new byte[1024];
                int length = _Connection.Receive(source);

                byte[] target = new byte[length];
                for (int i = 0; i < target.Length; i++)
                {
                    target[i] = source[i];
                }

                if (null != DataRecv)
                {
                    DataRecv(target);
                }
            }
        }
    }
}
