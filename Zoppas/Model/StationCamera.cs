namespace Zoppas.Model
{
    using System.Threading;
    using System;
    
    public class StationCamera : Station
    {
        static int _CalibrationFrame;  // 标定模式下支架号是固定的

        private readonly byte _CameraID;  // 相机编号
        private int _CurrentCount;  // 当前执行的项目编号
        Vision _Vision;

        public StationCamera(AutoResetEvent autoResetEvent, Vision vision, byte cameraID) : base(autoResetEvent)
        {
            _CameraID = cameraID;
            _CurrentCount = 0;
            _Vision = vision;
        }

        public override void Init(ICommunication[] communication)
        {
            base.Init(communication);
        }

        protected sealed override void OnReceive()
        {
            string order = String.Empty;

            // 根据系统模式来调用视觉检测程序编号
            switch (Global.SystemMode)
            {
                case SYSTEM_MODE.NONE:
                    break;

                case SYSTEM_MODE.CALIBRATION:
                    switch (_CameraID)
                    {
                        case 0:
                            _CalibrationFrame = Frame;
                            break;

                        default:
                            break;
                    }
                    order = _CameraID.ToString() + ',' + Global.CalibrationProjects[_CameraID][_CalibrationFrame, 0];
                    break;

                case SYSTEM_MODE.AUTO:
                    if (_CameraID == 1)
                    {
                        order = _CameraID.ToString() + "," + (Frame + Global.CountProjects[_CameraID][_CurrentCount] - 1).ToString();
                    }
                    else
                    {
                        order = _CameraID.ToString() + "," + (Global.CountProjects[_CameraID][_CurrentCount]).ToString();
                    }
                    // 重置计数
                    if (++_CurrentCount > Global.CountProjects[_CameraID].Length - 1)
                    {
                        _CurrentCount = 0;
                    }
                    break;

                default:
                    break;
            }
            
            _Vision.Send(order);
            _AutoResetEvent.WaitOne();
            Send();
        }

        protected sealed override void Send()
        {
            base.Send();
        }
    }
}
