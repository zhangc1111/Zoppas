namespace Zoppas.Model
{
    using System.Collections.Generic;

    public class ItemMulti : Item
    {
        public ItemMulti(string name, double min, double max) : base(name, min, max)
        {

        }

        public override void Excute(List<double> data)
        {
            base.Excute(data);

            _IsOK = data.MultiMin(_Max, _Min, out _Result);
        }
    }
}
