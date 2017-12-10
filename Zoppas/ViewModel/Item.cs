namespace Zoppas.ViewModel
{
    using GalaSoft.MvvmLight;
    public class Item : ObservableObject
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Item()
        {

        }

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

        private string _Value;
        public string Value
        {
            get { return _Value; }
            set
            {
                _Value = string.Format("{0:N2}", value);
                RaisePropertyChanged("Value");
            }
        }

        private bool _Result;
        public bool Result
        {
            get { return _Result; }
            set
            {
                _Result = value;
                RaisePropertyChanged("Result");
            }
        }

        private string _Min;
        public string Min
        {
            get { return _Min; }
            set
            {
                _Min = value;
                RaisePropertyChanged("Min");
            }
        }

        private string _Max;
        public string Max
        {
            get { return _Max; }
            set
            {
                _Max = value;
                RaisePropertyChanged("Max");
            }
        }
    }
}
