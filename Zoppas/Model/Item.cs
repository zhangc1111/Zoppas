/// <summary>
/// 检测项
/// </summary>
namespace Zoppas.Model
{
    using System;
    using System.Collections.Generic;

    public class Item
    {
        /// <summary>
        /// 检测项名称
        /// </summary>
        protected string _Name;
        /// <summary>
        /// 检测项下限
        /// </summary>
        protected double _Min;
        /// <summary>
        /// 检测项上限
        /// </summary>
        protected double _Max;
        /// <summary>
        /// 检测值
        /// </summary>
        protected double? _Result;
        public double? Result
        {
            get { return _Result; }
            private set
            {
                _Result = value;
            }
        }
        /// <summary>
        /// 检测结果
        /// </summary>
        protected bool? _IsOK;
        public bool? IsOK
        {
            get { return _IsOK; }
            protected set
            {
                _IsOK = value;
            }
        }

        private Item() { }

        public Item(string name, double min, double max) : this()
        {
            _Name = name;
            _Max = max;
            _Min = min;
        }

        // 检测方法
        public virtual void Excute(List<double> data)
        {
            if (data == null || data.Count == 0)
                throw new NullReferenceException("数据空.");

            IsOK = data.Default(_Max, _Min, out _Result);
        }

        // 清空结果
        public virtual void Clear()
        {
            _IsOK = null;
            _Result = null;
        }
    }
}
