namespace Zoppas.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ADLINK
    {
        const ushort CARD_TYPE = DASK.PCI_7230;
        const byte COUNT_INPUT_MOTOR = 8;
        const byte COUNT_OUTPUT_MOTOR = 8;
        const byte COUNT_OUTPUT_LIGHT = 2;

        // 上升沿触发
        static Dictionary<IO_INPUT_MOTOR, bool> m_DictFlagInputMotor;
        static Dictionary<IO_INPUT_MOTOR, ICommunication> m_DictComInputMotor;
        static Dictionary<IO_OUTPUT_MOTOR, ICommunication> m_DictComOutputMotor;
        static Dictionary<IO_OUTPUT_MOTOR, bool> m_DictFlagOutputMotor;

        static IO_INPUT_MOTOR[] m_InputMotors;
        static IO_OUTPUT_MOTOR[] m_OutputMotors;

        /// <summary>
        /// true状态刷button false状态刷finish
        /// </summary>
        public static bool EnButtonAndFinish;

        static ushort m_Number;
        static bool _IsRegistered;

        /// <summary>
        /// 完成信号的回复
        /// </summary>
        public static bool FinishIO { get; set; } = false;

        /// <summary>
        /// 按钮信号的回复
        /// </summary>
        public static bool ButtonIO { get; set; } = false;

        public static void Init(IO[] inputMotors, IO[] outputMotors)
        {
            try
            {
                DASK.Register_Card(CARD_TYPE, m_Number);
                _IsRegistered = true;
            }
            catch (Exception)
            {
                Logger.Fatal("ADLINK card error.");
                _IsRegistered = false;
            }

            InitInputMotor(inputMotors);
            InitOutputMotor(outputMotors);
            Start();
        }

        public static void Start()
        {
            if (!_IsRegistered) return;
            new Task(() => { Monitor(); }).Start();
        }

        static void InitInputMotor(IO[] inputMotors)
        {
            m_DictComInputMotor = new Dictionary<IO_INPUT_MOTOR, ICommunication>();
            m_InputMotors = new IO_INPUT_MOTOR[COUNT_INPUT_MOTOR];
            m_DictFlagInputMotor = new Dictionary<IO_INPUT_MOTOR, bool>();

            for (int i = 0; i < COUNT_INPUT_MOTOR; i++)
            {
                m_InputMotors[i] = (IO_INPUT_MOTOR)i;
                m_DictFlagInputMotor[m_InputMotors[i]] = false;
                m_DictComInputMotor[m_InputMotors[i]] = (i <= inputMotors.Length) ? inputMotors[i] : null;
            }
        }

        static void InitOutputMotor(IO[] outputMotors)
        {
            m_DictComOutputMotor = new Dictionary<IO_OUTPUT_MOTOR, ICommunication>();
            m_DictFlagOutputMotor = new Dictionary<IO_OUTPUT_MOTOR, bool>();
            m_OutputMotors = new IO_OUTPUT_MOTOR[COUNT_OUTPUT_MOTOR];

            for (int i = 0; i < COUNT_OUTPUT_MOTOR; i++)
            {
                m_OutputMotors[i] = (IO_OUTPUT_MOTOR)i;
                m_DictFlagOutputMotor[(IO_OUTPUT_MOTOR)i] = false;
                m_DictComOutputMotor[m_OutputMotors[i]] = (i <= outputMotors.Length) ? outputMotors[i] : null;
            }
        }

        public static void trigger(byte port)
        {
            uint value = 1;
            DASK.DO_WritePort(m_Number, port, value);
        }

        private static void Monitor()
        {
            // DASK 读取
            uint m_Read = 0;
            // DASK 写入
            uint m_Write = 0;

            while (true)
            {
                Thread.Sleep(1);

                DASK.DI_ReadPort(m_Number, 0, out m_Read);
                CheckSignal(ref m_Read, ref m_Write);
            }
        }

        private static void CheckSignal(ref uint read, ref uint write)
        {
            BitArray ba = Int32ToBitArray(read);

            if (EnButtonAndFinish)
            {
                #region 轮询ButtonIO
                if (ButtonIO)
                {
                    Send(true, 3, write);
                }
                else
                {
                    Send(false, 3, write);
                }
                #endregion
            }
            else
            {
                #region 轮询FinishIO
                if (FinishIO)
                {
                    Send(true, 4, write);
                }
                else
                {
                    Send(false, 4, write);
                }
                #endregion
            }

            for (int i = 0; i < ba.Length; i++)
            {
                if (ba[i])
                {
                    if (!m_DictFlagInputMotor[(IO_INPUT_MOTOR)i])
                    {
                        //if(i == 6)
                        //{
                        (m_DictComInputMotor[(IO_INPUT_MOTOR)i] as IO).Trigger();
//#if DEBUG
//                        Console.WriteLine("IO {0} triggered.", i);
//#endif
                        m_DictFlagInputMotor[(IO_INPUT_MOTOR)i] = true;
                        //}

                        #region simulate io
                        //Thread.Sleep(100);
                        //m_DictComOutputMotor[(IO_OUTPUT_MOTOR)i].Output("1");
                        #endregion
                    }
                    else
                    {
                        if ((m_DictComOutputMotor[(IO_OUTPUT_MOTOR)i] as IO).Enable)
                        {
                            if (!m_DictFlagOutputMotor[(IO_OUTPUT_MOTOR)i])
                            {
                                if(i == 4) { }
                                write = Send(true, i, write);
                                m_DictFlagOutputMotor[(IO_OUTPUT_MOTOR)i] = true;
//#if DEBUG
//                                Console.WriteLine("IO {0} send.", i);
//#endif
                            }
                        }
                        else
                        {
                            m_DictFlagOutputMotor[(IO_OUTPUT_MOTOR)i] = false;
                        }
                    }
                }
                else
                {
                    if (i < COUNT_OUTPUT_MOTOR)
                    {
                        if (m_DictFlagInputMotor[(IO_INPUT_MOTOR)i])
                        {
                            write = Send(false, i, write);
                            (m_DictComOutputMotor[(IO_OUTPUT_MOTOR)i] as IO).Output("0");
                            m_DictFlagInputMotor[(IO_INPUT_MOTOR)i] = false;
//#if DEBUG
//                            Console.WriteLine("IO {0} reset.", i);
//#endif
                        }
                    }
                }
            }
        }

        static uint Send(bool b, int index, uint value)
        {
            BitArray baWrite = Int32ToBitArray(value);
            baWrite[index] = b;
            value = BitArrayToInt32(baWrite);
            DASK.DO_WritePort(m_Number, 0, value);

            return value;
        }

        private static uint BitArrayToInt32(BitArray baWrite)
        {
            uint value = 0;
            BitArray ba = baWrite;

            for (int i = 0; i < ba.Length; i++)
            {
                if (ba[i])
                {
                    value += (uint)1 << i;
                }
            }

            return value;
        }

        private static BitArray Int32ToBitArray(uint value)
        {
            int convertBase = 2;
            string binary = Convert.ToString(value, convertBase);
            char[] bits = binary.ToCharArray();
            char[] newBits = new char[bits.Length];
            for (int i = 0; i < bits.Length; i++)
            {
                newBits[i] = bits[bits.Length - 1 - i];
            }
            BitArray ba = new BitArray(16);

            for (int i = 0; i < newBits.Length; i++)
            {
                if (newBits[i] == '1')
                {
                    ba[i] = true;
                }
                else
                {
                    ba[i] = false;
                }
            }

            return ba;
        }
    }
}
