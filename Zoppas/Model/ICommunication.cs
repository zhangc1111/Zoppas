namespace Zoppas.Model
{
    using System;
    using System.Threading.Tasks;

    public interface ICommunication
    {
        event Action Input;
        void Output(object order);
    }

    public class IO : ICommunication
    {
        public event Action Input;

        public bool Enable { get; private set; }

        public void Trigger()
        {
            new Task(() => { if (null != Input) Input(); }).Start();
        }

        public void Output(object order)
        {
            string str = order as string;
            if ("1" == str)
            {
                Enable = true;
            }
            else
            {
                Enable = false;
            }
        }
    }
}
