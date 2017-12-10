namespace Zoppas.Model
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;

    public class Vision
    {
        object _Locker;
        List<byte> _BufferSocket;  // tcp缓存 处理粘包问题
        SocketClient _Socket;
        AutoResetEvent[] _AutoResetEventSmalls;
        Queue<string> _QueueData;  // 数据队列
        Queue<string> _QueueOrder;  // 信号队列 发送给vitex
        bool _IsLast;

        public bool IsLast
        {
            get { return _IsLast; }
        }

        Vision()
        {
            _IsLast = false;
            _Socket = new SocketClient("127.0.0.1", 6000);
            _BufferSocket = new List<byte>();
            _Locker = new object();
            _QueueData = new Queue<string>();
            _QueueOrder = new Queue<string>();
        }

        public Vision(AutoResetEvent[] evtSmalls) : this()
        {
            _AutoResetEventSmalls = evtSmalls;
        }

        public void Connect()
        {
            _Socket.DataRecv += OnDataRecv;
            _Socket.Connect();
            StartVitex();
            new Task(() => SendOrderToVitex(), TaskCreationOptions.LongRunning).Start();
        }

        private void StartVitex()  // 发送START给vitex
        {
            if (_Socket.IsConnected)
            {
                string order = "|>START\r\n";
                byte[] buffer = System.Text.Encoding.ASCII.GetBytes(order);
                _Socket.Send(buffer);
            }
        }

        public void StopVitex()  // 发送STOP给vitex
        {
            if (_Socket.IsConnected)
            {
                string order = "|>STOP\r\n";
                byte[] buffer = System.Text.Encoding.ASCII.GetBytes(order);
                _Socket.Send(buffer);
            }
        }

        /// <summary>
        /// 获取视觉检测结果
        /// </summary>
        public Dictionary<int, List<double>> GetResult()
        {
            Dictionary<int, List<double>> dict = new Dictionary<int, List<double>>();

            //if (r_CountProject != m_Queue.Count)
            //throw new Exception("Vision 条数异常");

            while (_QueueData.Count > 0)
            {
                string result = _QueueData.Peek();
                if(result != null)
                {
                    result = _QueueData.Dequeue();
                    string[] items = result.Split(';');
                    foreach (string item in items)
                    {
                        string[] str = item.Split(':');
                        int key = Int32.Parse(str[0]);
                        double value = Double.Parse(str[1]);
                        if (dict.ContainsKey(key))
                        {
                            dict[key].Add(value);
                        }
                        else
                        {
                            dict.Add(key, new List<double>());
                            dict[key].Add(value);
                        }
                    }
                }
            }
            return dict;
        }

        #region 获取检测数据
        private void OnDataRecv(byte[] obj)
        {
            int index = 0;
            string result = String.Empty;
            int length = obj.Length;
            while (index < length)
            {
                if (obj[index] != 0x0a)
                {
                    _BufferSocket.Add(obj[index]);
                }
                else
                {
                    result = System.Text.Encoding.ASCII.GetString(_BufferSocket.ToArray());
                    _BufferSocket.Clear();
                    HandleResult(result);

                    string info = String.Format("Recv from vitex: {0}", result);
                    if (info.Length > 10)
                    {
                        //Logger.Info(info);
                    }
                }
                index++;
            }
        }

        private void HandleResult(string result)
        {
            // 把末尾的\r去掉
            string trimCR = result.Substring(0, result.Length);
            if (result.Length > 5)
            {
                new Task((s) => { SplitResult((string)s); }, trimCR).Start();
            }
        }

        private void SplitResult(string str)
        {
            string[] results = str.Split(',');
            if (results.Length == 5)
            {
                ProcessResult(results[4]);
                ContinueWork(results[1]);
            }
            else
            {
                //Log.Info("分解长度不等于5");
            }
        }

        private void ContinueWork(string str)
        {
            switch (str)
            {
                case "0":
                    _AutoResetEventSmalls[1].Set();
                    break;
                case "1":
                    _AutoResetEventSmalls[2].Set();
                    break;
                case "2":
                    _AutoResetEventSmalls[5].Set();
                    break;
                case "3":
                    _AutoResetEventSmalls[6].Set();
                    break;
                default:
                    break;
            }
        }

        private void ProcessResult(string str)
        {
            _QueueData.Enqueue(str);
        }
    #endregion

        public void Send(string value)
        {
            string order = String.Empty;
            string setProject = String.Empty;
            string[] values = value.Split(new char[] { ',' });
            int cameraID = Int32.Parse(values[0]);
            int projectID = Int32.Parse(values[1]);

            string setCamera = "|>SET CAMERA " + cameraID.ToString() + "\r\n";
            setProject = "|>SET CAMERA.PRODUCT.INDEX " + projectID.ToString() + "\r\n";
            order = setCamera + setProject + "|>SET CAMERA.ONESHOT\r\n";

            lock (_Locker)
            {
                _QueueOrder.Enqueue(order);
            }
        }

        private void SendOrderToVitex()
        {
            while (true)
            {
                string order = String.Empty;
                lock (_Locker)
                {
                    if (_QueueOrder.Count > 0)
                    {
                        order = _QueueOrder.Peek();
                        if (order != null)
                        {
                            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(order);
                            _Socket.Send(buffer);
                            _QueueOrder.Dequeue();
                            Thread.Sleep(50);
#if DEBUG
                            //Console.WriteLine("TCP {0}", order + ":" + m_QueueOrder.Count);
#endif
                        }
                    }
                }
            }
        }
    }
}

