namespace Zoppas.ViewModel
{
    using System;
    using System.Collections.Generic;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.CommandWpf;
    using Zoppas.Model;
    using System.Collections.ObjectModel;

    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// 状态栏信息
        /// </summary>
        string _Status;
        public string Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                RaisePropertyChanged("Status");
            }
        }
        /// <summary>
        /// 低壶参数
        /// </summary>
        private List<Parameter> _Parameters;
        public List<Parameter> Parameters
        {
            get { return _Parameters; }
        }

        /// <summary>
        /// 标定参数
        /// </summary>
        List<Parameter> _CalibrationParameters;
        public List<Parameter> CalibrationParameters
        {
            get { return _CalibrationParameters; }
        }

        private DateTime _Time;  // 界面日期选择
        public DateTime Time
        {
            get { return _Time; }
            set { _Time = value; }
        }

        private TYPE_COFFEE _Type;  // 界面型号选择
        public TYPE_COFFEE Type
        {
            get { return _Type; }
            set
            {
                _Type = value;
            }
        }

        private string _Item;  // 界面检测项选择
        public string Item
        {
            get { return _Item; }
            set { _Item = value; }
        }

        private AsyncObservableCollection<ViewProduct> _Products;  // 产品集合
        public AsyncObservableCollection<ViewProduct> Products
        {
            get { return _Products; }
        }

        private AsyncObservableCollection<Record> _Records;  // 数据库查询
        public AsyncObservableCollection<Record> Records
        {
            get { return _Records; }
        }

        private Manager _Manager;

        private RelayCommand _InitCommand;  // 初始化COMMAND
        public RelayCommand InitCommand
        {
            get { return _InitCommand; }
        }

        private RelayCommand _QueryCommand;  // 查询COMMAND
        public RelayCommand QueryCommand
        {
            get { return _QueryCommand; }
        }

        private RelayCommand _LoadFactoryParametersCommand;  // 导入工厂参数COMMAND
        public RelayCommand LoadFactoryParametersCommand
        {
            get { return _LoadFactoryParametersCommand; }
        }

        private RelayCommand _SaveFileParametersCommand;  // 导出参数配置COMMAND
        public RelayCommand SaveFileParametersCommand
        {
            get { return _SaveFileParametersCommand; }
        }

        private RelayCommand _LoadFileParametersCommand;  // 导入参数配置COMMAND
        public RelayCommand LoadFileParametersCommand
        {
            get { return _LoadFileParametersCommand; }
        }

        private RelayCommand _CloseMainWindowCommand;  // 关闭主窗体COMMAND
        public RelayCommand CloseMainWindowCommand
        {
            get { return _CloseMainWindowCommand; }
        }

        private ObservableCollection<Item> _Items;
        public ObservableCollection<Item> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                RaisePropertyChanged("Items");
            }
        }

        public MainViewModel()
        {
            _Manager = Manager.Instance;

            // 从配置文件导入参数表
            //Global.LoadFactory(_Manager.Type);
            //Global.ParameterChanged += OnParameterChanged;
            //Global.Deserialize(TYPE_COFFEE.T_7319);
            _Parameters = Global.ListItemParameter;
            _CalibrationParameters = Global.ListItemParameterCalibration;
            //Global.Serialize(TYPE_COFFEE.T_7319);
            //for (int i = 0; i < Global.ListItemParameter.Count; i++)
            //{
            //    _Parameters.Add(new Parameter()
            //    {
            //        Name = Global.ListItemName[i],
            //        FactoryMin = Global.ListItemParameter[i].StdMin.ToString(),
            //        FactoryMax = Global.ListItemParameter[i].StdMax.ToString()
            //    });
            //}
            //Global.Serialize(_Manager.Type); 
            //Global.Deserialize(_Manager.Type);
            //for (int i = 0; i < Global.ListItemParameter.Count; i++)
            //{
            //    _Parameters[i].UserMin = Global.ListItemParameter[i].StdMin.ToString();
            //    _Parameters[i].UserMax = Global.ListItemParameter[i].StdMax.ToString();
            //}
            _Products = new AsyncObservableCollection<ViewProduct>();
            _Records = new AsyncObservableCollection<Record>();
            _Type = _Manager.Type;
            _QueryCommand = new RelayCommand(() => { _Manager.QueryDatabase(Time, Type, Item); });
            _InitCommand = new RelayCommand(_Manager.Init);
            _LoadFileParametersCommand = new RelayCommand(() => { Global.Deserialize(_Manager.Type); });
            _SaveFileParametersCommand = new RelayCommand(() => { Global.Serialize(_Manager.Type); });
            _CloseMainWindowCommand = new RelayCommand(() => { _Manager.Close(); });
            _Manager.UpdateMainViewProducts += OnUpdateMainViewProducts;
            _Manager.UpdateMainViewRecords += OnUpdateMainViewRecords;
            _Manager.UpdateViewCalibrationResults += new Action<List<string>>(OnUpdateViewCalibrationResults);
            _Items = new ObservableCollection<Item>();
            for (int i = 0; i < Global.ListItemNameCalibration.Count; i++)
            {
                _Items.Add(new Item { Name = Global.ListItemNameCalibration[i], Min = Global.ListItemParameterCalibration[i].UserMin.ToString(), Max = Global.ListItemParameterCalibration[i].UserMax.ToString() });
            }
            _Manager.UpdateMainViewStatus += (s) => { Status = s; };
        }

        private void OnUpdateViewCalibrationResults(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                _Items[i].Value = list[i];
                _Items[i].Result = (double.Parse(list[i]) > Global.ListItemParameterCalibration[i].UserMin && double.Parse(list[i]) < Global.ListItemParameterCalibration[i].UserMax) ? true : false;
            }
        }

        private void OnParameterChanged()
        {
            for (int i = 0; i < Global.ListItemParameter.Count; i++)
            {
                //_Parameters[i].UserMin = Global.ListItemParameter[i].StdMin.ToString();
                //_Parameters[i].UserMax = Global.ListItemParameter[i].StdMax.ToString();
            }
        }

        private void OnUpdateMainViewRecords(string[] records)
        {
            Record record = new Record(records[0], records[1], records[2], records[3], records[4], records[5]);
            Records.Add(record);
        }

        private void OnUpdateMainViewProducts(List<Product> products)
        {
            Products.Clear();
            for (int i = 0; i < products.Count; i++)
            {
                Products.Add(new ViewProduct
                {
                    ID = products[i].ID,
                    Frame = products[i].Frame,
                    OP10 = true,
                    OP20 = products[i].OPs[1],
                    OP30 = products[i].OPs[2],
                    OP40 = true,
                    OP50 = true,
                    OP60 = products[i].OPs[5],
                    OP70 = products[i].OPs[6],
                    Result = products[i].Result,
                });
            }
        }
    }
}