namespace Zoppas.Model
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using GalaSoft.MvvmLight;

    public class OP : ObservableObject
    {
        public OP()
        {
            _Results = new bool?[8];
        }

        private bool?[] _Results;
        [IndexerName("Item")]
        public bool? this[int index]
        {
            get
            {
                if (index < 8)
                {
                    return _Results[index];
                }
                return null;
            }
            set
            {
                if (index < 8)
                {
                    _Results[index] = value;
                    RaisePropertyChanged("Item[]");
                }
            }
        }
    }
}
