namespace Zoppas.ViewModel
{
    using GalaSoft.MvvmLight;

    public class ViewProduct : ObservableObject
    {
        public ViewProduct()
        {

        }

        private string _ID;
        public string ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                RaisePropertyChanged("ID");
            }
        }

        private int _Frame;
        public int Frame
        {
            get { return _Frame; }
            set
            {
                _Frame = value;
                RaisePropertyChanged("Frame");
            }
        }

        private bool? _OP10;
        public bool? OP10
        {
            get { return _OP10; }
            set
            {
                _OP10 = value;
                RaisePropertyChanged("OP10");
            }
        }

        private bool? _OP20;
        public bool? OP20
        {
            get { return _OP20; }
            set
            {
                _OP20 = value;
                RaisePropertyChanged("OP20");
            }
        }

        private bool? _OP30;
        public bool? OP30
        {
            get { return _OP30; }
            set
            {
                _OP30 = value;
                RaisePropertyChanged("OP30");
            }
        }

        private bool? _OP40;
        public bool? OP40
        {
            get { return _OP40; }
            set
            {
                _OP40 = value;
                RaisePropertyChanged("OP40");
            }
        }

        private bool? _OP50;
        public bool? OP50
        {
            get { return _OP50; }
            set
            {
                _OP50 = value;
                RaisePropertyChanged("OP50");
            }
        }

        private bool? _OP60;
        public bool? OP60
        {
            get { return _OP60; }
            set
            {
                _OP60 = value;
                RaisePropertyChanged("OP60");
            }
        }

        private bool? _OP70;
        public bool? OP70
        {
            get { return _OP70; }
            set
            {
                _OP70 = value;
                RaisePropertyChanged("OP70");
            }
        }

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
    }
}
