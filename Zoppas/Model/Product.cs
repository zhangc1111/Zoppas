namespace Zoppas.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using GalaSoft.MvvmLight;

    public class Product : ObservableObject
    {
        private Product()
        {
            _OPs = new OP();
            _Result = null;
            _ListValue = new List<double?>();
            _ListCompensateValue = new List<double>();
            _ListResult = new List<bool?>();
            for (int i = 0; i < Global.DictItemID.Count; i++)
            {
                _ListValue.Add(null);
                _ListResult.Add(null);
            }
            _ID = DateTime.Now.ToString("yyyyMMddHHmmss");
            _Items = new Dictionary<int, List<double>>();
            _Compensation = new Dictionary<int, List<double>>();
        }

        public Product(byte currentOP, int frame) : this()
        {
            _CurrentOP = currentOP;
            _Frame = frame;
        }

        private string _ID;
        private bool? _Result;
        private int _Frame;
        private byte _CurrentOP;
        private OP _OPs;
        private Dictionary<int, List<double>> _Items;  // 每个检测项原始数据
        private Dictionary<int, List<double>> _Compensation;  // 补偿量

        public Dictionary<int, List<double>> Items
        {
            get { return _Items; }
        }

        public Dictionary<int, List<double>> Compensation
        {
            get { return _Compensation; }
        }

        /// <summary>
        /// 咖啡壶编号
        /// 年月日时分秒
        /// </summary>
        public string ID
        {
            get { return _ID; }
            private set
            {
                _ID = value;
                RaisePropertyChanged("ID");
            }
        }
        /// <summary>
        /// 当前工位
        /// </summary>
        public byte CurrentOP
        {
            get { return _CurrentOP; }
            set { _CurrentOP = value; }
        }
        /// <summary>
        /// 支架
        /// </summary>
        public int Frame
        {
            get { return _Frame; }
        }
        /// <summary>
        /// 工位检测结果
        /// </summary>
        public OP OPs
        {
            get { return _OPs; }
        }
        /// <summary>
        /// 检测项值未补偿
        /// </summary>
        private List<double?> _ListValue;
        public List<double?> ListValue
        {
            get { return _ListValue; }
        }

        /// <summary>
        /// 检测项值已补偿
        /// </summary>
        private List<double> _ListCompensateValue;
        public List<double> ListCompensateValue
        {
            get { return _ListCompensateValue; }
        }

        /// <summary>
        /// 检测项结果对应字典
        /// </summary>
        private List<bool?> _ListResult;
        public List<bool?> ListResult
        {
            get { return _ListResult; }
        }
        /// <summary>
        /// 总结果
        /// </summary>
        public bool? Result
        {
            get { return _Result; }
            set
            {
                _Result = value;
                RaisePropertyChanged("Result");
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

