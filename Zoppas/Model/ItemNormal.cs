namespace Zoppas.Model
{
    using System.Collections.Generic;

    public class ItemNormal : Item
    {
        public ItemNormal(string name, double min, double max) : base(name, min, max)
        {

        }

        public override void Excute(List<double> data)
        {
            base.Excute(data);

            // 直接等于第一个值
            _Result = data[0];
            if (_Result <= _Max && _Result >= _Min)
            {
                IsOK = true;
            }
            else
            {
                IsOK = false;
            }
        }
    }
}
