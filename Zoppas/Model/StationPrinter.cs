namespace Zoppas.Model
{
    using System;
    using System.Threading;
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.IO;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public class StationPrinter : Station, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // 打印机状态
        // 16 3b 45 06 01 0d 打印机喷印完成
        // 16 3b 56 00 00/01 0d 外部文本发送失败/成功
        public enum STATUS
        {
            // 断开
            DISCONNECTED,
            // 连接
            CONNECTED,
            // 正在发送外部文本
            SEND_EXT_TEXT,
            // 外部文本发送失败
            RECV_EXT_FAIL,
            // 外部文本发送成功
            RECV_EXT_SUCCESS,
            // 喷印完成
            RECV_PRT_FIN,
        }

        // 断包串口数据
        private List<byte> m_Buffer;

        // COM端口号
        private readonly string r_PortName;

        private SerialPort m_SerialPort;
        private STATUS m_Status = STATUS.DISCONNECTED;
        // 喷码机接收缓冲区成功
        private bool m_IsReceived;
        // 喷码机喷印成功
        private bool m_IsPrinted;

        public STATUS Status
        {
            get { return m_Status; }
            // 设置状态 绑定界面打印机状态
            private set
            {
                m_Status = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Status"));
                }
            }
        }

        // 构造
        public StationPrinter(AutoResetEvent autoResetEvent, string port) : base(autoResetEvent)
        {
            m_Buffer = new List<byte>();
            r_PortName = port;
        }

        // 重写Station.Init
        public override void Init(ICommunication[] communication)
        {
            // 初始化串口
            InitSerialPort();
            // 基类
            base.Init(communication);
        }

        private void InitSerialPort()
        {
            try
            {
                m_SerialPort = new SerialPort(r_PortName);
                m_SerialPort.BaudRate = 38400;
                m_SerialPort.DataReceived += OnDataReceived;
                m_SerialPort.Open();

                // COM端口连接成功
                Status = STATUS.CONNECTED;
            }
            catch (IOException ex)
            {
                // COM端口连接失败
                Status = STATUS.DISCONNECTED;
            }
            finally
            {
                if (null == m_SerialPort)
                {
                    Console.WriteLine("");
                }
            }
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // 缓冲区字节长度
            int length = m_SerialPort.BytesToRead;
            // 按次序读取6个字节一组,剩余的放到下一组
            while (length-- > 0)
            {
                // 读取缓冲区
                byte[] buffer = new byte[1];
                m_SerialPort.Read(buffer, 0, 1);
                m_Buffer.Add(buffer[0]);

                if (m_Buffer.Count == 6)
                {
                    CheckBuffer(m_Buffer.ToArray());
                    m_Buffer.Clear();
                }
            }
        }

        private void CheckBuffer(byte[] buffer)
        {
            if (buffer[0] == 0x16 && buffer[1] == 0x3b && buffer[5] == 0x0d)
            {
                switch (buffer[2])
                {
                    case 0x56:
                        // 成功
                        if (buffer[4] == 0x01)
                        {
                            m_IsReceived = true;
                            Console.WriteLine("写入喷码机缓冲区成功.");
                            _AutoResetEvent.Set();
                        }
                        // 失败
                        else
                        {
                            m_IsReceived = false;
                            Console.WriteLine("写入喷码机缓冲区失败.");
                        }
                        break;

                    case 0x45:
                        m_IsPrinted = true;
                        Console.WriteLine("喷印完成.");
                        break;

                    default:
                        Console.WriteLine("未知功能号.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("喷码机返回数据异常.");
                foreach (var item in buffer)
                {
                    Console.WriteLine(item.ToString("x2") + " ");
                }
            }
        }

        // 电机到位 IO信号触发打印工位
        protected sealed override void OnReceive()
        {
            base.OnReceive();
            // 开始喷码
            Product p = null;
            //foreach (var item in Manager.Instance.Coffees)
            //{
            //    if (item.CurrentOP == 6)
            //    {
            //        p = item;
            //    }
            //}
            // 更新显示的壶信息
            //Manager.Instance.DisplayProduct[0] = Manager.Instance.DisplayProduct[1];
            //Manager.Instance.DisplayProduct[1] = (Product)p.Clone();
            //Print(p);
            // IO信号触发电机动作
            //Send();
            // 存储到数据库
            //SaveToDatabase(p);
        }

        protected sealed override void Send()
        {
            base.Send();
        }

        // 给打印机发送要打印的字符
        private void Print(Product p)
        {
            PrintExecute(p);
        }

        private void PrintExecute(Product p)
        {
            // COM口没有打开
            if (!m_SerialPort.IsOpen)
            {
                Logger.Fatal("Com1 is not open.");
            }

            // 找到当前位置是6的壶号
            if (p.OPs[1] != null && p.OPs[2] != null && p.OPs[5] != null && p.OPs[4] != null && p.OPs[6] != null)
            {
                while (true)
                {
                    //PrintID(p);
                    _AutoResetEvent.WaitOne();
                    if (m_IsReceived)
                    {
                        // 重置
                        m_IsReceived = false;
                        break;
                    }
                    else
                    {
                        // 等待 再次下发
                        Thread.Sleep(500);
                    }
                }
                while (true)
                {
                    //PrintResult(p);
                    _AutoResetEvent.WaitOne();
                    if (m_IsReceived)
                    {
                        // 重置
                        m_IsReceived = false;
                        break;
                    }
                    else
                    {
                        // 等待再次下发
                        Thread.Sleep(500);
                    }
                }
            }
            else
            {
                throw new Exception("有工位没有检测到结果.");
            }
        }

        // 喷码第一行 壶的型号+壶的编号
        //private void PrintID(Product p)
        //{
        //    // 报头
        //    byte[] bytesTitle = new byte[] { 0x16, 0x2a, 0x01, 0x02, 0x50, 0x56, 0x01, 0x00, 0x13 };
        //    int lengthTitle = bytesTitle.Length;
        //    // 编号
        //    string id = Manager.Instance._global.DictTypeName[Manager.Instance.m_Coffee] + " " + p.ID;
        //    byte[] bytesID = System.Text.Encoding.ASCII.GetBytes(id);
        //    int lengthID = bytesID.Length;
        //    // 结束符
        //    byte[] bytesEnd = new byte[] { 0x0d };
        //    int lengthEnd = bytesEnd.Length;
        //    // 拼接
        //    byte[] buffer = new byte[lengthTitle + lengthID + lengthEnd];
        //    for (int i = 0; i < lengthTitle; i++)
        //    {
        //        buffer[i] = bytesTitle[i];
        //    }
        //    for (int j = 0; j < lengthID; j++)
        //    {
        //        buffer[lengthTitle + j] = bytesID[j];
        //    }
        //    for (int k = 0; k < lengthEnd; k++)
        //    {
        //        buffer[lengthTitle + lengthID + k] = bytesEnd[k];
        //    }
        //    Console.WriteLine("发送编号 " + id);
        //    // 发送
        //    m_SerialPort.Write(buffer, 0, buffer.Length);
        //}

        // 喷码结果
        //private void PrintResult(Product p)
        //{
        //    // 报头
        //    byte[] bytesTitle = new byte[] { 0x16, 0x2a, 0x01, 0x02, 0x50, 0x56, 0x02, 0x00, 0x0b };
        //    int lengthTitle = bytesTitle.Length;
        //    // 结果
        //    string result = String.Empty;
        //    // 喷码支架号
        //    result += p.Frame.ToString() + " ";
        //    foreach (var item in p.DictResult.Keys)
        //    {
        //        if (p.DictResult[item] != null)
        //        {
        //            if (!(bool)p.DictResult[item])
        //            {
        //                result += Manager.Instance._global.DictItemPrint[item];
        //            }
        //        }
        //    }
        //    // 如果结果长度小于0x0b 后面补空格
        //    if(result.Length < 0x0b)
        //    {
        //        int length = (int)0x0b - result.Length;
        //        for (int h = 0; h < length; h++)
        //        {
        //            result += " ";
        //        }
        //    }
        //    byte[] bytesResult = System.Text.Encoding.ASCII.GetBytes(result);
        //    int lengthID = bytesResult.Length;
        //    // 结束符
        //    byte[] bytesEnd = new byte[] { 0x0d };
        //    int lengthEnd = bytesEnd.Length;
        //    // 拼接
        //    byte[] buffer = new byte[lengthTitle + lengthID + lengthEnd];
        //    for (int i = 0; i < lengthTitle; i++)
        //    {
        //        buffer[i] = bytesTitle[i];
        //    }
        //    for (int j = 0; j < lengthID; j++)
        //    {
        //        buffer[lengthTitle + j] = bytesResult[j];
        //    }
        //    for (int k = 0; k < lengthEnd; k++)
        //    {
        //        buffer[lengthTitle + lengthID + k] = bytesEnd[k];
        //    }
        //    Console.WriteLine("发送结果 " + result);
        //    // 发送
        //    m_SerialPort.Write(buffer, 0, buffer.Length);
        //}

        private void SaveToDatabase(Product p)
        {
            //if (Manager.Instance.Coffees.Count == 0)
            //    return;
            // 插入记录
            //List<insertRecordStr> list = new List<insertRecordStr>();
            //string id = p.ID;
            //string value = String.Empty;
            //foreach (var item in p.DictValue.Keys)
            //{
            //    insertRecordStr record = new insertRecordStr() { insertItemName = "item" + item.ToString(), insertItemValue = p.DictValue[item].ToString() };
            //    list.Add(record);
            //    value += record.insertItemName;
            //    value += record.insertItemValue;
            //}
            //bool? result = true;
            //foreach (var item in p.DictResult.Keys)
            //{
            //    result &= p.DictResult[item];
            //}
            //string r = (result == true) ? "1" : "0";
            //MySQLTool.MySql.InsertRow(id, "7319", r, list);

            Console.WriteLine("数据库存储完成.");
        }

        private void OrderPrintOn()
        {
            if (!m_SerialPort.IsOpen)
                return;
            byte[] buffer = new byte[] { 0x16, 0x2a, 0x01, 0x02, 0x50, 0x50, 0x0d };
            m_SerialPort.Write(buffer, 0, buffer.Length);
        }

        private void OrderPrintOff()
        {
            if (!m_SerialPort.IsOpen)
                return;
            byte[] buffer = new byte[] { 0x16, 0x2a, 0x01, 0x02, 0x50, 0x51, 0x0d };
            m_SerialPort.Write(buffer, 0, buffer.Length);
        }
    }
}
