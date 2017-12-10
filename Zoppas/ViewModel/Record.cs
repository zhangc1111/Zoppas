/// <summary>
/// record from mysql database
/// </summary>

namespace Zoppas.ViewModel
{
    using System;
    using GalaSoft.MvvmLight;
    using Model;

    public class Record : ObservableObject
    {
        private string _Date;
        public string Date
        {
            get { return _Date; }
            private set
            {
                _Date = value;
                RaisePropertyChanged("Date");
            }
        }

        private string _Type;
        public string Type
        {
            get { return _Type; }
            private set
            {
                _Type = value;
                RaisePropertyChanged("Type");
            }
        }

        private string _Total;
        public string Total
        {
            get { return _Total; }
            private set
            {
                _Total = value;
                RaisePropertyChanged("Total");
            }
        }

        private string _Pass;
        public string Pass
        {
            get { return _Pass; }
            private set
            {
                _Pass = value;
                RaisePropertyChanged("Pass");
            }
        }

        private string _Ratio;
        public string Ratio
        {
            get { return _Ratio; }
            private set
            {
                _Ratio = value;
                RaisePropertyChanged("Ratio");
            }
        }

        private string _FailItem;  // 该检测项不合格计数
        public string FailItem
        {
            get { return _FailItem; }
            private set
            {
                _FailItem = value;
                RaisePropertyChanged("FailItem");
            }
        }

        public Record(string date, string type, string total, string pass, string ratio, string failItem)
        {
            Date = date;
            Type = type;
            Total = total;
            Pass = pass;
            Ratio = ratio;
            FailItem = failItem;
        }
    }
}
