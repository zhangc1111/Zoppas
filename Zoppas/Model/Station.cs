namespace Zoppas.Model
{
    using System;
    using System.Threading;

    // 工位
    public abstract class Station
    {
        // PLC 通讯 IO
        protected ICommunication m_CommInput;
        protected ICommunication m_ComOutput;
        protected AutoResetEvent _AutoResetEvent;

        // 支架号 
        // 相机工位根据支架号来调用不同的程序号
        public int Frame { get; set; }  

        protected static object Locker;

        Station()
        {

        }

        public Station(AutoResetEvent autoResetEvent) : this()
        {
            _AutoResetEvent = autoResetEvent;
        }

        public virtual void Init(ICommunication[] communication)
        {
            m_CommInput = communication[0];
            m_ComOutput = communication[1];
            m_CommInput.Input += OnReceive;
        }

        protected virtual void OnReceive()
        {
            //throw new NotImplementedException();
        }

        // IO 触发PLC转到下一个位置
        protected virtual void Send()
        {
            m_ComOutput.Output("1");
        }
    }

    public class StationEmpty : Station
    {
        public StationEmpty(AutoResetEvent autoResetEvent) : base(autoResetEvent)
        {

        }
    }
}

