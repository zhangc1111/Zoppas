namespace Zoppas.Model
{
    using System.Collections.Generic;

    public class Processor
    {
        private string _Frame;
        public string Frame
        {
            get { return _Frame; }
            set { _Frame = value; }
        }
        /// <summary>
        /// 是否校准
        /// 每次收到支架号的时候设置为空
        /// </summary>
        private bool? _IsCalibrated;
        public bool? IsCalibrated
        {
            get { return _IsCalibrated; }
            set
            {
                _IsCalibrated = value;
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        public Processor()
        {
            _IsCalibrated = null;
            _Frame = string.Empty;
        }

        public List<string> Process(Dictionary<int, List<double>> dict)
        {
            List<string> data = new List<string>();
            Item item = null;
            for (int i = 0; i < Global.ListItemIDCalibration.Count; i++)
            {
                switch (Global.ListItemTypeCalibration[i])
                {
                    case TYPE_ITEM.NORMAL:
                        item = new ItemNormal(Global.ListItemNameCalibration[i], Global.ListItemParameterCalibration[i].UserMin, Global.ListItemParameterCalibration[i].UserMax);
                        break;
                    case TYPE_ITEM.MULTI:
                        item = new ItemMulti(Global.ListItemNameCalibration[i], Global.ListItemParameterCalibration[i].UserMin, Global.ListItemParameterCalibration[i].UserMax);
                        break;
                    case TYPE_ITEM.DIFF:
                        item = new ItemDiff(Global.ListItemNameCalibration[i], Global.ListItemParameterCalibration[i].UserMin, Global.ListItemParameterCalibration[i].UserMax);
                        break;
                    case TYPE_ITEM.BETWEEN:
                        item = new ItemBtween(Global.ListItemNameCalibration[i], Global.ListItemParameterCalibration[i].UserMin, Global.ListItemParameterCalibration[i].UserMax);
                        break;
                    default:
                        break;
                }
                item.Excute(dict[Global.DictItemCalibration[i]]);
                //if (Global.ListItemID[i] == 7)
                //{
                //    item.Excute(plc[Global.ListItemID[i]]);
                //}
                //else
                //{
                //    item.Excute(vision[Global.ListItemID[i]]);
                //}

                if (null == _IsCalibrated)
                {
                    _IsCalibrated = item.IsOK;
                }
                else
                {
                    _IsCalibrated &= item.IsOK;
                }
                data.Add(item.Result.ToString());
            }
            return data;
        }
    }
}
