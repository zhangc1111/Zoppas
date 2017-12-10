namespace Zoppas.Model
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class Manager
    {
        #region Instance

        private static Manager _Instance;
        public static Manager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Manager();
                return _Instance;
            }
        }

        #endregion

        #region Events

        public event Action ClearSubViewItems;
        public event Action<List<string>> UpdateViewCalibrationResults;
        public event Action<string> UpdateMainViewStatus;
        public event Action<List<Product>> UpdateMainViewProducts;  // 更新主界面 产品
        public event Action<string[]> UpdateMainViewRecords;  //  更新主界面 数据库记录
        public event Action<List<double?>, List<bool?>> UpdateSubViewItems;  // 更新次界面 检测项
        public event Action<bool?, int> UpdateSubViewProductFinish;  // 更新次界面 一个新产品检测完成 
        
        #endregion

        #region Fields

        static int _FrameOfOP10 = 0;  // OP10工位支架号
        PLC _Plc;
        Vision _Vision;
        TYPE_COFFEE _Type;  // 咖啡壶型号
        AutoResetEvent[] _AutoResetEvents;  // 视觉锁
        IO[] _IOInput;
        IO[] _IOOutput;
        Station[] _Stations;
        object _LockerDict;
        List<Item> _Items;  // 检测项
        List<Product> _Products;  // 产品队列
        bool _EnNewInspect = true;  // 开始新一轮检测
        Processor _CalibrationProcessor;  // 处理标定结果
        Saver _Saver;
        
        #endregion

        #region Property Type

        public TYPE_COFFEE Type
        {
            get { return _Type; }
        }

        #endregion

        #region Constructors

        Manager()
        {
            AppDomain.CurrentDomain.UnhandledException += (obj, e) => { Logger.Info(e.ExceptionObject.ToString()); };
            _Saver = new Saver();
            //_Type = TYPE_COFFEE.T_7319;
            _Products = new List<Product>();
            _LockerDict = new object();
            _AutoResetEvents = new AutoResetEvent[8]
            {
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
            };
            _IOInput = new IO[8] { new IO(), new IO(), new IO(), new IO(), new IO(), new IO(), new IO(), new IO() };
            _IOOutput = new IO[8] { new IO(), new IO(), new IO(), new IO(), new IO(), new IO(), new IO(), new IO() };
            _Plc = new PLC(1);

            _Vision = new Vision(_AutoResetEvents);
            _Stations = new Station[8]
            {
                new StationEmpty(_AutoResetEvents[0]),
                new StationCamera(_AutoResetEvents[1], _Vision, 0),
                new StationCamera(_AutoResetEvents[2], _Vision, 1),
                new StationEmpty(_AutoResetEvents[3]),
                new StationEmpty(_AutoResetEvents[4]),
                new StationCamera(_AutoResetEvents[5], _Vision, 2),
                new StationCamera(_AutoResetEvents[6], _Vision, 3),
                new StationPrinter(_AutoResetEvents[7], "COM1"),
            };

            _CalibrationProcessor = new Processor();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            _Plc.ChangeCoffeeType += new Action<TYPE_COFFEE>(OnChangeCoffeeType);  // 系统默认，暂时不用
            _Plc.StartPopCoffeePot += new Action(OnStartPopCoffeePot);
            _Plc.PushButton += new Action(OnPushButton);
            _Plc.CalibrationFinished += new Action(OnCalibrationFinished);
            _Plc.InspectFinished += new Action(OnInspectFinished);
            _Plc.StartAutoMode += new Action<int>(OnStartAutoMode);
            _Plc.StartCalibrationMode += new Action<int>(OnStartCalibrationMode);
            _Plc.Connect();

            _Vision.Connect();

            ADLINK.Init(_IOInput, _IOOutput);

            _Stations[7].Init(new ICommunication[] { _IOInput[7], _IOOutput[7] });
            _Stations[1].Init(new ICommunication[] { _IOInput[1], _IOOutput[1] });
            _Stations[2].Init(new ICommunication[] { _IOInput[2], _IOOutput[2] });
            _Stations[5].Init(new ICommunication[] { _IOInput[5], _IOOutput[5] });
            _Stations[6].Init(new ICommunication[] { _IOInput[6], _IOOutput[6] });

            int result = -1;
            try
            {
                result = MySQLTool.MySql.MySqlConn("localhost", "root", "lj12345", "coffeepot");
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
            finally
            {
                if (result == -1)
                {
                    Logger.Info("Database connect fail.");
                }
                if (result == 0)
                {
                    Logger.Info("Database connect success.");
                }
            }
        }

        /// <summary>
        /// 查询数据库
        /// </summary>
        /// <param name="date"></param>
        /// <param name="type"></param>
        /// <param name="itemName"></param>
        public void QueryDatabase(DateTime date, TYPE_COFFEE type, string itemName)
        {
            int countFailItem = 0;
            string[] results = MySQLTool.MySql.CheckResultSelect(date, type.ToString());
            List<float> listItems = MySQLTool.MySql.SelectItem(date, type.ToString(), itemName);
            int i = Int32.Parse(Regex.Replace(itemName, @"^[A-Za-z]+", string.Empty));
            Global.ChangeCoffeeType(type);
            foreach (var item in listItems)
            {
                if (item < Global.ListItemParameter[i - 1].UserMin || item > Global.ListItemParameter[i - 1].UserMax)
                {
                    countFailItem++;
                }
            }
            Global.ChangeCoffeeType((TYPE_COFFEE)Enum.Parse(typeof(TYPE_COFFEE), type.ToString()));
            string[] statistic = new string[results.Length + 1];
            for (int j = 0; j < results.Length; j++)
            {
                statistic[j] = results[j];
            }
            statistic[statistic.Length - 1] = countFailItem.ToString();
            if (UpdateMainViewRecords != null)
            {
                UpdateMainViewRecords(statistic);
            }
        }

        #endregion

        #region Helpers

        void OnStartCalibrationMode(int frame)
        {
            if(UpdateMainViewStatus != null)
            {
                UpdateMainViewStatus("启动标定模式，支架号" + frame.ToString());
            }
            Global.SystemMode = SYSTEM_MODE.CALIBRATION;
            _FrameOfOP10 = frame;
            _CalibrationProcessor.Frame = frame.ToString();
            for (int i = 0; i < _Stations.Length; i++)
            {
                _Stations[i].Frame = frame - i > 0 ? frame - i : frame - i + 8;
            }
        }

        void OnStartAutoMode(int frame)
        {
            if(UpdateMainViewStatus != null)
            {
                UpdateMainViewStatus("启动自动模式，支架号" + frame.ToString());
            }
            Global.SystemMode = SYSTEM_MODE.AUTO;
            _FrameOfOP10 = frame;
            for (int i = 0; i < _Stations.Length; i++)
            {
                _Stations[i].Frame = frame - i > 0 ? frame - i : frame - i + 8;
            }
        }

        void OnCalibrationFinished()
        {
            if (UpdateMainViewStatus != null)
            {
                UpdateMainViewStatus("标定完成");
            }
            Dictionary<int, List<double>> dict = GetDict();
            List<string> results = _CalibrationProcessor.Process(dict);
            SaveResult(results);
            if (null != UpdateViewCalibrationResults)
            {
                UpdateViewCalibrationResults(results);
            }
            _EnNewInspect = true;
            _Plc.EnPush = true;
        }

        void SaveResult(List<string> results)
        {
            _Saver.Save(results, _CalibrationProcessor.Frame, (bool)_CalibrationProcessor.IsCalibrated);
        }

        void OnInspectFinished()
        {
            if (UpdateMainViewStatus != null)
            {
                UpdateMainViewStatus("自动完成");
            }
            if (_EnNewInspect)
            {
                Logger.Info("重复收到Finish信号");
                return;
            }

            Dictionary<int, List<double>> dict = GetDict();
            _EnNewInspect = true;
            InsertItemToProduct(dict);
            CalculateProductItems();
            UpdateProducts();
            _Plc.EnPush = true;
            Clear();
            SaveDatabase();
        }

        /// <summary>
        /// 汇总所有检测结果
        /// </summary>
        /// <returns></returns>
        Dictionary<int, List<double>> GetDict()
        {
            Dictionary<int, List<double>> dictAll = new Dictionary<int, List<double>>();
            Dictionary<int, List<double>> dictPLC = new Dictionary<int, List<double>>();
            Dictionary<int, List<double>> dictVision = new Dictionary<int, List<double>>();
            try
            {
                dictPLC = _Plc.GetResult();
                foreach (var item in dictPLC.Keys)
                {
                    dictAll.Add(item, dictPLC[item]);
                }
            }
            catch (Exception)
            {
                dictPLC.Clear();
            }
            try
            {
                dictVision = _Vision.GetResult();

                foreach (var item in dictVision.Keys)
                {
                    dictAll.Add(item, dictVision[item]);
                }
            }
            catch (Exception)
            {
                dictVision.Clear();
            }
            return dictAll;
        }

        void InsertItemToProduct(Dictionary<int, List<double>> dict)
        {
            for (int i = 0; i < _Products.Count; i++)
            {
                foreach (var item in Global.ListOP[_Products[i].CurrentOP])
                {
                    if (!_Products[i].Items.ContainsKey(item))
                    {
                        _Products[i].Items.Add(item, dict[item]);
                    }
                }
                if (_Products[i].CurrentOP == 1)
                {
                    if (dict.ContainsKey(22))
                    {
                        try
                        {
                            if (!_Products[i].Compensation.ContainsKey(22))
                            {
                                _Products[i].Compensation.Add(22, dict[22]);
                            }
                            else
                            {
                                Logger.Info("Already contains key 22");
                            }
                        }
                        catch (Exception)
                        {
                            throw new Exception("InsertItemToProduct error");
                        }

                    }
                    else
                    {
                        Logger.Info("item 22 not exist.");
                    }
                    if (dict.ContainsKey(23))
                    {
                        try
                        {
                            if (!_Products[i].Compensation.ContainsKey(23))
                            {
                                _Products[i].Compensation.Add(23, dict[23]);
                            }
                            else
                            {
                                Logger.Info("Already contains key 23");
                            }
                        }
                        catch (Exception)
                        {
                            throw new Exception("InsertItemToProduct error");
                        }
                    }
                    else
                    {
                        Logger.Info("item 23 not exist.");
                    }
                }
            }
        }

        void CalculateProductItems()
        {
            if (_Products.Count == 0) return;
            Product product = _Products[0];
            if (product.CurrentOP == 7)
            {
                foreach (var item in product.Items.Keys)
                {
                    int key = Global.DictItemID[item];
                    Item productItem = null;

                    switch (Global.ListItemType[key])
                    {
                        case TYPE_ITEM.NORMAL:
                            productItem = new ItemNormal(Global.ListItemParameter[key].Name, Global.ListItemParameter[key].UserMin, Global.ListItemParameter[key].UserMax);
                            break;
                        case TYPE_ITEM.DIFF:
                            productItem = new ItemDiff(Global.ListItemParameter[key].Name, Global.ListItemParameter[key].UserMin, Global.ListItemParameter[key].UserMax);
                            break;
                        case TYPE_ITEM.MULTI:
                            productItem = new ItemMulti(Global.ListItemParameter[key].Name, Global.ListItemParameter[key].UserMin, Global.ListItemParameter[key].UserMax);
                            break;
                        case TYPE_ITEM.BETWEEN:
                            Logger.Fatal("Create item between fail.");
                            break;
                        default:
                            Logger.Fatal("Create item default fail.");
                            break;
                    }
                    if (item == 8 || item == 9 || item == 21)
                    {
                        // 这三个检测项加上检测项1的结果
                        product.Items[item][0] += (double)product.ListValue[0];
                    }
                    if (item == 6)
                    {
                        // 6加上22补偿
                        //product.Items[item][0] += product.Compensation[22][0];
                    }
                    if (item == 12)
                    {
                        // 12加上23补偿
                        product.Items[item][0] += product.Compensation[23][0];
                    }
                    productItem.Excute(product.Items[item]);
                    product.ListValue[Global.DictItemID[item]] = productItem.Result;
                    product.ListResult[Global.DictItemID[item]] = productItem.IsOK;
                }
            }
        }

        /// <summary>
        /// 更新及存储数据
        /// </summary>
        void UpdateProducts()
        {
            // 更新OP结果
            for (int i = 0; i < _Products.Count; i++)
            {
                for (int j = 0; j < Global.ListOP.Count; j++)
                {
                    bool? op = null;
                    foreach (var item in Global.ListOP[j])
                    {
                        if (op.HasValue)
                        {
                            op &= _Products[i].ListResult[Global.DictItemID[item]];
                        }
                        else
                        {
                            op = _Products[i].ListResult[Global.DictItemID[item]];
                        }
                    }
                    _Products[i].OPs[j] = op;
                }
                if (_Products[i].OPs[1] != null && _Products[i].OPs[2] != null && _Products[i].OPs[5] != null && _Products[i].OPs[6] != null)
                {
                    if (_Products[i].OPs[1] == true && _Products[i].OPs[2] == true && _Products[i].OPs[5] == true && _Products[i].OPs[6] == true)
                    {
                        _Products[i].Result = true;
                    }
                    else
                    {
                        _Products[i].Result = false;
                    }
                }
                else
                {
                    _Products[i].Result = null;
                }
            }

            if (_Products.Count >= 0)
            {
                if (UpdateMainViewProducts != null)
                {
                    UpdateMainViewProducts(_Products);
                }
                if (_Products.Count == 0)
                    return;
                if (_Products[0].CurrentOP == 7)
                {
                    if (UpdateSubViewProductFinish != null)
                    {
                        UpdateSubViewProductFinish(_Products[0].Result, _Products[0].Frame);
                    }
                }
                if (UpdateSubViewItems != null)
                {
                    if (_Products.Count == 0)
                    {
                        List<double?> value = new List<double?>();
                        List<bool?> result = new List<bool?>();
                        UpdateSubViewItems(value, result);
                    }
                    else
                    {
                        UpdateSubViewItems(_Products[0].ListValue, _Products[0].ListResult);
                    }
                }
            }
        }

        /// <summary>
        /// 清理数据
        /// </summary>
        private void Clear()
        {
            //_Items = new List<Item>();
        }

        /// <summary>
        /// 存储数据库
        /// </summary>
        void SaveDatabase()
        {
            if (_Products.Count == 0) return;
            List<insertRecordStr> list = new List<insertRecordStr>();
            string id = String.Empty;
            string type = _Type.ToString();
            string result = String.Empty;
            Product product = _Products[0];
            if (product.CurrentOP == 7)
            {
                id = product.ID;
                for (int i = 0; i < product.ListValue.Count; i++)
                {
                    list.Add(new insertRecordStr { insertItemName = "Item" + (i + 1).ToString(), insertItemValue = product.ListValue[i].ToString() });
                }
                if (product.Result.HasValue)
                {
                    result = (bool)product.Result ? "1" : "0";
                }
                MySQLTool.MySql.InsertRow(id, type, result, list);
            }
        }

        #endregion

        void OnPushButton()
        {
            if(UpdateMainViewStatus != null)
            {
                UpdateMainViewStatus("按钮");
            }
            // PushButton和InspectFinished信号配对接收
            // 如果一轮检测没有完成，再次按PushButton没有作用
            if (!_EnNewInspect)
                return;

            switch (Global.SystemMode)
            {
                //  没有设置系统模式时，按钮没有作用
                case SYSTEM_MODE.NONE:
                    break;

                //  校准模式
                case SYSTEM_MODE.CALIBRATION:
                    for (int i = 0; i < _Stations.Length; i++)
                    {
                        _Stations[i].Frame = ++_Stations[i].Frame > 8 ? _Stations[i].Frame - 8 : _Stations[i].Frame;
                    }
                    break;

                //  自动模式
                case SYSTEM_MODE.AUTO:
                    Product product = null;
                    for (int i = 0; i < _Stations.Length; i++)
                    {
                        _Stations[i].Frame = ++_Stations[i].Frame > 8 ? _Stations[i].Frame - 8 : _Stations[i].Frame;
                    }
                    switch (Global.Workmode)
                    {
                        case WORK_MODE.PUSH:
                            product = new Product(0, _FrameOfOP10);
                            _Products.Add(product);
                            if (_Products.Count >= 7)
                            {
                                Global.Workmode = WORK_MODE.WORK;
                            }
                            break;
                        case WORK_MODE.WORK:
                            _Products.Remove(_Products[0]);
                            product = new Product(0, _FrameOfOP10);
                            _Products.Add(product);
                            break;
                        case WORK_MODE.POP:
                            _Products.Remove(_Products[0]);
                            if (_Products.Count == 0)
                            {
                                // 最后一个壶清完
                                OnInspectFinished();
                                if(ClearSubViewItems != null)
                                {
                                    ClearSubViewItems();
                                }
                                Global.Workmode = WORK_MODE.PUSH;
                            }
                            break;
                        default:
                            break;
                    }
                    _FrameOfOP10 = ++_FrameOfOP10 > 8 ? _FrameOfOP10 - 8 : _FrameOfOP10;
                    foreach (var item in _Products)
                    {
                        item.CurrentOP = (byte)((item.CurrentOP + 1) % 8);
                    }
                    break;

                default:
                    break;
            }
            _EnNewInspect = false;
        }

        /// <summary>
        /// 开始清壶模式
        /// </summary>
        void OnStartPopCoffeePot()
        {
            Global.Workmode = WORK_MODE.POP;
            if(UpdateMainViewStatus != null)
            {
                UpdateMainViewStatus("启动清壶模式");
            }
        }

        /// <summary>
        /// 切换壶的种类，根据种类再切换相关参数
        /// </summary>
        /// <param name="type"></param>
        private void OnChangeCoffeeType(TYPE_COFFEE type)
        {
            _Type = type;
            Global.ChangeCoffeeType(type);
        }

        public void Close()
        {
            _Vision.StopVitex();
            Environment.Exit(0);
        }
    }
}
