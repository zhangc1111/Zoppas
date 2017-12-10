namespace Zoppas.Model
{
    using System.Collections.Generic;

    public class ItemDiff : Item
    {
        // 最大值
        private double? _High;
        // 最小值
        private double? _Low;

        public ItemDiff(string name, double min, double max) : base(name, min, max)
        {

        }

        public override void Excute(List<double> data)
        {
            base.Excute(data);

            // 取所有数据中最大最小的差值
            _IsOK = data.Difference(_Max, _Min, out _High, out _Low, out _Result);
        }
    }
}
