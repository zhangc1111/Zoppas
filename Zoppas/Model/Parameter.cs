namespace Zoppas.Model
{
    using GalaSoft.MvvmLight;
    using System;

    [Serializable]
    public class Parameter : ObservableObject
    {
        private Parameter()
        {

        }

        public Parameter(string unit) : this()
        {
            _Unit = unit;
        }

        /// <summary>
        /// 检测项名称
        /// </summary>
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                RaisePropertyChanged("Name");
            }
        }

        /// <summary>
        /// 标准下公差
        /// </summary>
        private double _FactoryMin;
        public double FactoryMin
        {
            get { return _FactoryMin; }
            set
            {
                _FactoryMin = value;
                RaisePropertyChanged("FactoryMin");
            }
        }

        /// <summary>
        /// 标准上公差
        /// </summary>
        private double _FactoryMax;
        public double FactoryMax
        {
            get { return _FactoryMax; }
            set
            {
                _FactoryMax = value;
                RaisePropertyChanged("FactoryMax");
            }
        }

        /// <summary>
        /// 用户下公差
        /// </summary>
        private double _UserMin;
        public double UserMin
        {
            get { return _UserMin; }
            set
            {
                _UserMin = value;
                RaisePropertyChanged("UserMin");
            }
        }

        /// <summary>
        /// 用户上公差
        /// </summary>
        private double _UserMax;
        public double UserMax
        {
            get { return _UserMax; }
            set
            {
                _UserMax = value;
                RaisePropertyChanged("UserMax");
            }
        }

        /// <summary>
        /// 单位
        /// </summary>
        private string _Unit;
        public string Unit
        {
            get { return _Unit; }
        }
    }
}

