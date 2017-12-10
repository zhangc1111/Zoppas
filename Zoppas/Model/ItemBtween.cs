namespace Zoppas.Model
{
    using System.Collections.Generic;

    public class ItemBtween : Item
    {
        private double? m_High;
        // 最小值
        private double? m_Low;

        public ItemBtween(string name, double min, double max) : base(name, min, max)
        {

        }

        public override void Excute(List<double> data)
        {
            base.Excute(data);

            // 取所有数据中最大最小的差值
            _IsOK = data.AllBetween(_Max, _Min, out _Result);
        }
    }
}
