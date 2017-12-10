namespace Zoppas.ViewModel
{
    using System;
    using System.Collections.Generic;
    using Model;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.CommandWpf;

    public class SubViewModel : ObservableObject
    {
        private List<ViewItem> _Items;  // 产品集合
        public List<ViewItem> Items
        {
            get { return _Items; }
        }
        private int _Total;  // 当天生产总数
        public int Total
        {
            get { return _Total; }
            set
            {
                _Total = value;
                RaisePropertyChanged("Total");

            }
        }
        private int _Pass;  // 合格数
        public int Pass
        {
            get { return _Pass; }
            set
            {
                _Pass = value;
                RaisePropertyChanged("Pass");
            }
        }

        private int _Fail;  // 失败数
        public int Fail
        {
            get { return _Fail; }
            set
            {
                _Fail = value;
                RaisePropertyChanged("Fail");
            }
        }

        private double _Ratio;  // 合格率
        public double Ratio
        {
            get { return _Ratio; }
            set
            {
                _Ratio = value;
                RaisePropertyChanged("Ratio");
            }
        }

        private string _Frame;  // 支架号
        public string Frame
        {
            get { return _Frame; }
            set
            {
                _Frame = value;
                RaisePropertyChanged("Frame");
            }
        }

        private byte _ItemValueVisibility;  //元素可见性
        public byte ItemValueVisibility
        {
            get
            {
                return _ItemValueVisibility;
            }
            set
            {
                _ItemValueVisibility = value;
                RaisePropertyChanged("ItemValueVisibility");
            }
        }

        private RelayCommand _HideItemsValueCommand;
        public RelayCommand HideItemsValueCommand
        {
            get { return _HideItemsValueCommand; }
        }

        private Manager _Manager;

        public SubViewModel()
        {
            _Frame = String.Empty;
            _Total = 0;
            _Pass = 0;
            _Fail = 0;
            _Ratio = 0;
            _Items = new List<ViewItem>();
            for (int i = 0; i < 16; i++)
            {
                _Items.Add(new ViewItem());
            }
            _Manager = Manager.Instance;
            _Manager.UpdateSubViewItems += OnUpdateItems;
            _Manager.UpdateSubViewProductFinish += OnUpdateProductFinish;
            _Manager.ClearSubViewItems += new Action(OnClearSubViewItems);
            _HideItemsValueCommand = new RelayCommand(() =>
           {
               if (_ItemValueVisibility == 0)
               {
                   ItemValueVisibility = 1;
               }
               else if(_ItemValueVisibility == 1)
               {
                   ItemValueVisibility = 0;
               }
               else
               {

               }
           });
        }

        private void OnClearSubViewItems()
        {
            for (int i = 0; i < _Items.Count; i++)
            {
                _Items[i].Name = String.Empty;
                _Items[i].Value = null;
                _Items[i].Result = null;
                _Items[i].Ratio = null;
                _Items[i].Pass = 0;
            }
        }

        private void OnUpdateProductFinish(bool? result, int frame)
        {
            Frame = frame.ToString();
            if (result.HasValue)
            {
                Total++;
                if ((bool)result)
                {
                    Pass++;
                }
                else
                {
                    Fail++;
                }
                Ratio = (double)Pass / (double)Total;
            }
            else
            {
                Logger.Fatal("Finish product's result is null.");
            }
        }

        private void OnUpdateItems(List<double?> value, List<bool?> result)
        {
            if(value.Count == 0)
            {
                for (int j = 0; j < _Items.Count; j++)
                {
                    _Items[j].Name = String.Empty;
                    _Items[j].Value = null;
                    _Items[j].Result = null;
                    _Items[j].Ratio = null;
                }
            }
            for (int i = 0; i < value.Count; i++)
            {
                _Items[i].Name = Global.ListItemParameter[i].Name;
                _Items[i].Value = value[i];
                _Items[i].Result = result[i];
                if (result[i].HasValue)
                {
                    if ((bool)result[i])
                    {
                        ++_Items[i].Pass;
                    }
                    _Items[i].Ratio = (double)_Items[i].Pass / Total;
                }
            }
        }
    }
}
