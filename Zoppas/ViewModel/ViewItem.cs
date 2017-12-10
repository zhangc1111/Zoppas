namespace Zoppas.ViewModel
{
    using GalaSoft.MvvmLight;

    /// <summary>
    /// 检测项
    /// </summary>
    public class ViewItem : ObservableObject
    {
        public ViewItem()
        {

        }

        /// <summary>
        /// Inspect item name
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
        /// Inspect item value
        /// </summary>
        private double? _Value;
        public double? Value
        {
            get
            {
                if (_Value.HasValue)
                {
                    return (double)(int)(_Value * 1000) / 1000;
                }
                else
                {
                    return _Value;
                }
            }
            set
            {
                _Value = value;
                RaisePropertyChanged("Value");
            }
        }
        /// <summary>
        /// Inspect item result 1
        /// </summary>
        private bool? _Result;
        public bool? Result
        {
            get { return _Result; }
            set
            {
                _Result = value;
                RaisePropertyChanged("Result");
            }
        }

        private double? _Ratio;  // 合格率
        public double? Ratio
        {
            get
            {
                if (_Ratio.HasValue)
                {
                    return (double)(int)(_Ratio * 1000) / 10;
                }
                return _Ratio;
            }
            set
            {
                _Ratio = value;
                RaisePropertyChanged("Ratio");
            }
        }

        private int _Pass;  // 合格数
        public int Pass
        {
            get
            {
                return _Pass;
            }
            set
            {
                _Pass = value;
                RaisePropertyChanged("Pass");
            }
        }
    }
}
