namespace Zoppas.Model
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using System.IO;
    using System;

    public static class Global
    {
        /// <summary>
        /// 当参数更换时通知界面更新
        /// </summary>
        public static event Action ParameterChanged;

        static Global()
        {
            Deserialize(TYPE_COFFEE.T_7319);
            _CountProjects = _CountProjects7319;
            _Workmode = WORK_MODE.PUSH;
            _SystemMode = SYSTEM_MODE.NONE;
            //_VisionCountPush = _VisionCountPush7319;
            //_VisionCountWork = _VisionCountWork7319;
            //_VisionCountPop = _VisionCountPop7319;
            _DictItemID = _DictItemID7319;
            _ListItemParameter = _ListItemParameter7319;
            _ListOP = _ListOP7319;
            _ListItemType = _ListItemType7319;
        }

        /// <summary>
        /// 相机执行的项目序列
        /// </summary>
        private static int[][] _CountProjects;
        public static int[][] CountProjects
        {
            get { return _CountProjects; }
        }

        /// <summary>
        /// 自动工作模式
        /// </summary>
        private static WORK_MODE _Workmode;
        public static WORK_MODE Workmode
        {
            get { return _Workmode; }
            set { _Workmode = value; }
        }

        static SYSTEM_MODE _SystemMode;
        public static SYSTEM_MODE SystemMode
        {
            get { return _SystemMode; }
            set { _SystemMode = value; }
        }

        private static List<string> _ListItemName;
        //public static List<string> ListItemName
        //{
        //    get { return _ListItemName; }
        //}

        private static Dictionary<int, int> _DictItemID;
        public static Dictionary<int, int> DictItemID
        {
            get { return _DictItemID; }
        }

        private static List<TYPE_ITEM> _ListItemType;
        public static List<TYPE_ITEM> ListItemType
        {
            get { return _ListItemType; }
        }

        private static List<Parameter> _ListItemParameter;
        public static List<Parameter> ListItemParameter
        {
            get { return _ListItemParameter; }
        }

        private static List<List<byte>> _ListOP;
        public static List<List<byte>> ListOP
        {
            get { return _ListOP; }
        }

        public static List<Parameter> ListItemParameter7319
        {
            get { return _ListItemParameter7319; }
        }

        /// <summary>
        /// 切换咖啡壶的种类
        /// </summary>
        public static void ChangeCoffeeType(TYPE_COFFEE type)
        {
            switch (type)
            {
                case TYPE_COFFEE.T_7319:
                    _Workmode = WORK_MODE.PUSH;
                    //_ListItemName = _ListItemName7319;
                    _DictItemID = _DictItemID7319;
                    _ListItemParameter = _ListItemParameter7319;
                    _ListOP = _ListOP7319;
                    _ListItemType = _ListItemType7319;
                    break;
                case TYPE_COFFEE.T_7320:
                    break;
                default:
                    throw new NotImplementedException("Unsupported type.");
            }
        }

        private static void NotifyView()
        {
            if (null != ParameterChanged)
            {
                ParameterChanged();
            }
        }

        public static void LoadFactory(TYPE_COFFEE type)
        {
            //ChangeCoffeeType(type);
            //NotifyView();
        }

        /// <summary>
        /// 保存检测项参数配置
        /// </summary>
        public static void Serialize(TYPE_COFFEE type)
        {
            //string currentDirectory = Directory.GetCurrentDirectory();
            string currentDirectory = @"E:\佐帕斯\Coffee\Zoppas\bin\Debug";
            File.Delete(currentDirectory + "\\" + type.ToString() + ".txt");
            FileStream fs = new FileStream(currentDirectory + "\\" + type.ToString() + ".txt", FileMode.Create);
            XmlSerializer serializer = new XmlSerializer(typeof(List<Parameter>));
            serializer.Serialize(fs, ListItemParameter);
            fs.Close();
        }

        /// <summary>
        /// 导入检测项参数配置
        /// </summary>
        public static void Deserialize(TYPE_COFFEE type)
        {
            //string currentDirectory = Directory.GetCurrentDirectory();
            string currentDirectory = @"E:\佐帕斯\Coffee\Zoppas\bin\Debug";
            FileStream fs = null;
            try
            {
                fs = new FileStream(currentDirectory + "\\" + type.ToString() + ".txt", FileMode.Open);
            }
            catch (Exception)
            {
                Logger.Fatal("Open file failed.");
                _ListItemParameter7319 = new List<Parameter>
        {
            { new Parameter("mm"){ Name = "平整度", FactoryMin = 0, FactoryMax = 0.5, UserMin = 0, UserMax = 0.5} },
            { new Parameter("mm"){ Name = "NTC孔处加热丝位置", FactoryMin = 39, FactoryMax = 45, UserMin = 39, UserMax = 45} },
            { new Parameter("mm"){ Name = "螺钉位置", FactoryMin = 52.5, FactoryMax = 53.5, UserMin = 52.5, UserMax = 53.5} },
            { new Parameter("度"){ Name = "螺钉角度", FactoryMin = 88.5, FactoryMax = 91.5, UserMin = 88.5, UserMax = 91.5} },
            { new Parameter("mm"){ Name = "左电极位置", FactoryMin = 98, FactoryMax = 104, UserMin = 98, UserMax = 104} },
            { new Parameter("mm"){ Name = "水嘴位置", FactoryMin = 85, FactoryMax = 87, UserMin = 85, UserMax = 87} },
            { new Parameter("mm"){ Name = "NTC孔直径", FactoryMin = 4.0, FactoryMax = 4.2, UserMin = 4.0, UserMax = 4.2} },
            { new Parameter("mm"){ Name = "NTC位置", FactoryMin = 31.65, FactoryMax = 32.25, UserMin = 31.65, UserMax = 32.25} },
            { new Parameter("mm"){ Name = "右电极位置", FactoryMin = 98, FactoryMax = 104, UserMin = 98, UserMax = 104} },
            { new Parameter("mm"){ Name = "温控器到加热管间隙", FactoryMin = 0.5, FactoryMax = 2, UserMin = 0.5, UserMax = 2} },
            { new Parameter("mm"){ Name = "热熔断器到加热管间隙", FactoryMin = 0.5, FactoryMax = 1.2, UserMin = 0.5, UserMax = 1.2} },
            { new Parameter("mm"){ Name = "2加热丝间隙", FactoryMin = 1, FactoryMax = 999, UserMin = 1, UserMax = 999} },
        };
                return;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(List<Parameter>));
            _ListItemParameter7319 = (List<Parameter>)serializer.Deserialize(fs);
            _ListItemParameter = _ListItemParameter7319;
            //ChangeCoffeeType(TYPE_COFFEE.T_7319);
            //NotifyView();
            fs.Close();
        }

        #region #1 PRODUCT 7319
        private static int[][] _CountProjects7319 =
        {
            new int[]{ 1, 2, 3, 4, 5, 6, 7 },
            new int[]{ 8, 16, 24 },
            new int[]{ 1, 1, 1, 1, 1, 1, 1 },
            new int[]{ 1, 2, 3, 4 },
        };

        private static byte[] _VisionCountPush7319 = { 7, 10, 10, 10, 17, 21, 21 };
        private static byte _VisionCountWork7319 = 21;
        private static byte[] _VisionCountPop7319 = { 0, 0, 4, 11, 11, 11, 14 };

        /// <summary>
        /// 对应的检测项下标
        /// </summary>
        private static List<List<byte>> _ListOP7319 = new List<List<byte>>()
        {
            { new List<byte>(){ } },
            { new List<byte>(){ 1 } },
            { new List<byte>(){ 6, 7, 8, 9, 11, 12, 21 } },
            { new List<byte>(){ } },
            { new List<byte>(){ 2 } },
            { new List<byte>(){ 16 } },
            { new List<byte>(){ 14, 15 } },
            { new List<byte>(){ } },
        };
        private static List<TYPE_ITEM> _ListItemType7319 = new List<TYPE_ITEM>
        {
            { TYPE_ITEM.DIFF },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.MULTI },
        };
        /// <summary>
        /// Inspect item
        /// </summary>
        private static Dictionary<int, int> _DictItemID7319 = new Dictionary<int, int>
        {
            { 1, 0 },
            { 2, 1 },
            { 6, 2 },
            { 7, 3 },
            { 8, 4 },
            { 9, 5 },
            { 11, 6 },
            { 12, 7 },
            { 21, 8 },
            { 14, 9 },
            { 15, 10 },
            { 16, 11 },
        };

        /// <summary>
        /// Inspect item parameter
        /// </summary>
        private static List<Parameter> _ListItemParameter7319;
        #endregion

        #region 标定块 #1

        /// <summary>
        /// 标定程序调用序列
        /// </summary>
        static readonly int[][,] _CalibrationProjects = new int[][,]
        {
            new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
            },
            new int[,]
            {
                { 0 },
                { 1 },
                { 2 },
                { 3 },
                { 4 },
                { 5 },
                { 6 },
                { 7 },
            },
            new int[,]
            {
                { 0 },
                { 0 },
                { 0 },
                { 0 },
                { 0 },
                { 0 },
                { 0 },
                { 0 },
            },
            new int[,]
            {
                { 0 },
                { 0 },
                { 0 },
                { 0 },
                { 0 },
                { 0 },
                { 0 },
                { 0 },
            },
        };
        public static int[][,] CalibrationProjects
        {
            get { return _CalibrationProjects; }
        }

        /// <summary>
        /// 工位对应标定检测项
        /// </summary>
        private static List<List<byte>> _ListOPCalibration = new List<List<byte>>()
        {
            { new List<byte>(){ } },
            { new List<byte>(){ 0, 1 } },
            { new List<byte>(){ 3, 4, 5, 6, } },
            { new List<byte>(){ 7 } },
            { new List<byte>(){ 2 } },
            { new List<byte>(){ 8 } },
            { new List<byte>(){ 9 } },
            { new List<byte>(){ } },
        };
        public static List<List<byte>> ListOPCalibration
        {
            get { return _ListOPCalibration; }
        }
        /// <summary>
        /// 标定项类型
        /// </summary>
        private static List<TYPE_ITEM> _ListItemTypeCalibration = new List<TYPE_ITEM>
        {
            { TYPE_ITEM.BETWEEN },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.NORMAL },
            { TYPE_ITEM.NORMAL },
        };
        private static Dictionary<int, int> _DictItemCalibration = new Dictionary<int, int>
        {
            { 0, 0 },
            { 1, 1 },
            { 2, 3 },
            { 3, 4 },
            { 4, 5 },
            { 5, 6 },
            { 6, 2 },
            { 7, 8 },
            { 8, 9 },
        };
        public static Dictionary<int,int> DictItemCalibration
        {
            get { return _DictItemCalibration; }
        }
        public static List<TYPE_ITEM> ListItemTypeCalibration
        {
            get { return _ListItemTypeCalibration; }
        }
        /// <summary>
        /// 标定项名称
        /// </summary>
        static List<string> _ListItemNameCalibration = new List<string>
        {
            { "OP20 #1" },
            { "OP20 #2" },
            { "OP30 #1" },
            { "OP30 #2" },
            { "OP30 #3" },
            { "OP30 #4" },
            { "OP50 #1" },
            { "OP60 #1" },
            { "OP70 #1" },
        };
        public static List<string> ListItemNameCalibration
        {
            get { return _ListItemNameCalibration; }
        }
        /// <summary>
        /// 标定项ID
        /// </summary>
        private static List<int> _ListItemIDCalibration = new List<int>
        {
            { 1 },
            { 2 },
            { 3 },
            { 4 },
            { 5 },
            { 6 },
            { 7 },
            { 8 },
            { 9 },
        };
        public static List<int> ListItemIDCalibration
        {
            get { return _ListItemIDCalibration; }
        }
        /// <summary>
        /// 标定项参数
        /// </summary>
        static List<Parameter> _ListItemParameterCalibration = new List<Parameter>
        {
            { new Parameter("mm"){ Name = "OP20 #1", FactoryMin = 7.95, FactoryMax = 8.05, UserMin = 7.95, UserMax = 8.05 } },
            { new Parameter("mm"){ Name = "OP20 #2", FactoryMin = 7.95, FactoryMax = 8.05, UserMin = 7.95, UserMax = 8.05 } },
            { new Parameter("mm"){ Name = "OP30 #1", FactoryMin = 49.95, FactoryMax = 50.05, UserMin = 49.95, UserMax = 50.05 } },
            { new Parameter("mm"){ Name = "OP30 #2", FactoryMin = 49.95, FactoryMax = 50.05, UserMin = 49.95, UserMax = 50.05 } },
            { new Parameter("mm"){ Name = "OP30 #3", FactoryMin = 4.95, FactoryMax = 5.05, UserMin = 4.95, UserMax = 5.05 } },
            { new Parameter("mm"){ Name = "OP30 #4", FactoryMin = 89.70, FactoryMax = 90.30, UserMin = 89.70, UserMax = 90.30 } },
            { new Parameter("mm"){ Name = "OP50 #1", FactoryMin = 43.05, FactoryMax = 43.09, UserMin = 43.05, UserMax = 43.09 } },
            { new Parameter("mm"){ Name = "OP60 #1", FactoryMin = 9.97, FactoryMax = 10.03, UserMin = 9.97, UserMax = 10.03 } },
            { new Parameter("mm"){ Name = "OP70 #1", FactoryMin = 9.97, FactoryMax = 10.03, UserMin = 9.97, UserMax = 10.03 } },
        };
        public static List<Parameter> ListItemParameterCalibration
        {
            get { return _ListItemParameterCalibration; }
        }
        #endregion
    }
}