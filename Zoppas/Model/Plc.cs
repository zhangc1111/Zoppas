// 90 自动模式 支架号
// 100 标定模式 支架号
// 80 所有工位完成信号

namespace Zoppas.Model
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;

    internal class PLC
    {
        /// <summary>
        /// 所有工位检测项目完成
        /// </summary>
        internal event Action InspectFinished;
        /// <summary>
        /// 开始清壶
        /// </summary>
        public event Action StartPopCoffeePot;
        /// <summary>
        /// 获取当前OP10工位的支架号
        /// </summary>
        //public event Action<int> GetFrame;
        /// <summary>
        /// 获取壶种类
        /// </summary>
        public event Action<TYPE_COFFEE> ChangeCoffeeType;
        /// <summary>
        /// 按下启动按钮
        /// </summary>
        public event Action PushButton;
        /// <summary>
        /// 标定完成
        /// </summary>
        public event Action CalibrationFinished;
        /// <summary>
        /// 开始自动模式并附带支架编号
        /// </summary>
        public event Action<int> StartAutoMode;
        /// <summary>
        /// 开始标定模式并附带支架编号
        /// </summary>
        public event Action<int> StartCalibrationMode;

        /// <summary>
        /// 启用按钮
        /// 当所有检测都完成时通过manager来更新
        /// </summary>
        private bool _EnPush;
        public bool EnPush
        {
            get { return _EnPush; }
            set { _EnPush = value; }
        }
        private bool _Pushed;
        private Queue<string> _Queue;
        private SocketClient _Socket;
        private bool? _EnFrame;

        private PLC()
        {
            _Queue = new Queue<string>();
            _Socket = new SocketClient("192.168.0.1", 2000);
            _EnPush = true;
        }

        public PLC(int count) : this()
        {
            new Task(() => { Loop(); }).Start();
        }

        public void Connect()
        {
            _Socket.Connect();
            _Socket.DataRecv += OnDataRecv;
        }

        /// <summary>
        /// 获取PLC检测的数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, List<double>> GetResult()
        {
            Dictionary<int, List<double>> dict = new Dictionary<int, List<double>>();

            while (_Queue.Count > 0)
            {
                string result = _Queue.Peek();
                if(result != null)
                {
                    result = _Queue.Dequeue();
                    string[] results = result.Split(':');
                    int key = Int32.Parse(results[0]);
                    double value = Double.Parse(results[1]);
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
            return dict;
        }

        void OnDataRecv(byte[] daba)
        {
            if(daba.Length != 6)
            {
                Logger.Info("PLC数据粘包");
            }
            string strOP = String.Empty;
            byte[] op = new byte[2];
            byte[] value = new byte[4];
            op[0] = daba[1];
            op[1] = daba[0];
            value[0] = daba[5];
            value[1] = daba[4];
            value[2] = daba[3];
            value[3] = daba[2];
            short OP = BitConverter.ToInt16(op, 0);
            float Value = BitConverter.ToSingle(value, 0);
            Logger.Info("收到PLC " + OP);
            int frame = 0;
            switch (OP)
            {
                // 清壶
                case 10:  
                    if (StartPopCoffeePot != null)
                    {
                        StartPopCoffeePot();
                    }
                    break;

                // 按钮按下
                case 20:  
                    _Pushed = true;
                    //_Socket.Send(new byte[] { 0x00, 0x01 });
                    ADLINK.EnButtonAndFinish = true;
                    ADLINK.ButtonIO = true;
                    Thread.Sleep(100);
                    ADLINK.ButtonIO = false;
                    break;

                // 获取2号检测项
                case 50:  
                    strOP = "2";
                    _Queue.Enqueue(strOP + ":" + Value.ToString());
                    break;

                // 标定完成
                case 70:  
                    if(CalibrationFinished != null) { CalibrationFinished(); }
                    break;

                // 所有工位完成
                case 80:  
                    //int type = Convert.ToInt32(Value);
                    //ChangeCoffeeType((TYPE_COFFEE)type);
                    if(InspectFinished != null) { InspectFinished(); }
                    //_Socket.Send(new byte[] { 0x00, 0x01 });
                    ADLINK.EnButtonAndFinish = false;
                    ADLINK.FinishIO = true;
                    Thread.Sleep(100);
                    ADLINK.FinishIO = false;
                    break;

                // 自动模式并且获取支架号
                case 90:
                    if (_EnFrame.HasValue && !(bool)_EnFrame)
                        return;

                    frame = Convert.ToInt32(Value);
                    if (StartAutoMode != null)
                    {
                        StartAutoMode(frame / 100);
                    }
                    _EnFrame = false;
                    break;

                // 标定模式并且获取支架号
                case 100:
                    if (_EnFrame.HasValue && (bool)_EnFrame)
                        return;

                    frame = Convert.ToInt32(Value);
                    if (StartCalibrationMode != null)
                    {
                        StartCalibrationMode(frame / 100);
                    }
                    _EnFrame = true;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 检测按钮信号
        /// </summary>
        private void Loop()
        {
            while (true)
            {
                Thread.Sleep(1);
                if (_EnPush)
                {
                    if (_Pushed)
                    {
                        if (null != PushButton)
                        {
                            Logger.Info("Recv Plc Button Signal");
                            PushButton();
                            _Pushed = false;
                            _EnPush = false;
                        }
                    }
                }
            }
        }
    }
}

