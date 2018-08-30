using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Common;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Device;
using ServerSuperIO.Device.Connector;
using ServerSuperIO.Protocol;
using ServerSuperIO.Protocol.Filter;
using ServerSuperIO.Service.Connector;
using ServerSuperIO.WebSocket;

namespace TestDeviceDriver
{
    public class DeviceSingletonDriver:RunDevice
    {
        private DeviceDyn _deviceDyn;
        private DevicePara _devicePara;
        private DeviceProtocol _protocol;
        public DeviceSingletonDriver()
            : base()
        {
            _devicePara = new DevicePara();
            _deviceDyn = new DeviceDyn();
            _protocol = new DeviceProtocol();
        }

        public override void Initialize(object obj)
        {
            this.Protocol.InitDriver(this.GetType(), null);

            //初始化设备参数信息
            
            //_devicePara.DeviceID = devid;//设备的ID必须先赋值，因为要查找对应的参数文件。
            //_devicePara.Load();

            ////初始化设备实时数据信息
            //_deviceDyn.DeviceID = devid;//设备的ID必须先赋值，因为要查找对应的实时数据文件。
            //_deviceDyn.Load();
        }

        public override IList<IRequestInfo> GetConstantCommand()
        {
            //return this.Protocol.DriverPackage<String,String>("0", "61", String.Empty,String.Empty);
            return null;
        }

        public override void Communicate(ServerSuperIO.Communicate.IResponseInfo info)
        {
            string hexs = BinaryUtil.ByteToHex(info.Data);
            OnDeviceRuningLog("接收>>" + hexs);

            byte[] cmds = this.Protocol.GetCommand(info.Data);
            CommandArray cr = (CommandArray)cmds[0];

            dynamic obj = this.Protocol.DriverAnalysis<byte[]>(cr.ToString(), info.Data, info.BigData);
            if (obj != null)
            {
                if (cr == CommandArray.RealTimeData)
                {
                    Dyn dyn = (Dyn)obj;

                    this.DeviceDynamic.DynamicData.Write("flow", dyn.Flow);
                    this.DeviceDynamic.DynamicData.Write("signal", dyn.Signal);

                    OnDeviceRuningLog("通讯正常，流量：" + dyn.Flow + ",信号：" + dyn.Signal);
                }
                else if (cr == CommandArray.FileData)
                {
                    OnDeviceRuningLog("文件存储路径：" + obj.ToString());
                }
                //else if (cr == CommandArray.BackControlCommand)
                //{
                //    if (_serviceCallback != null)
                //    {
                //        _serviceCallback.BeginInvoke(new string[] { this.DeviceParameter.DeviceCode, _channelKey }, null, null);
                //    }
                //}
            }
        }

        public override void CommunicateInterrupt(ServerSuperIO.Communicate.IResponseInfo info)
        {
            OnDeviceRuningLog("通讯中断");
        }

        public override void CommunicateError(ServerSuperIO.Communicate.IResponseInfo info)
        {
            OnDeviceRuningLog("通讯干扰");
        }

        public override void CommunicateNone()
        {
            OnDeviceRuningLog("通讯未知");
        }

        //public override void Alert()
        //{
        //    return;
        //}

        //public override void Save()
        //{
        //    try
        //    {
        //        _deviceDyn.Save();
        //    }
        //    catch (Exception ex)
        //    {
        //        OnDeviceRuningLog(ex.Message);
        //    }
        //}

        //public override void Show()
        //{
        //    //List<string> list=new List<string>();
        //    //list.Add(_devicePara.DeviceName);
        //    //list.Add(_deviceDyn.Dyn.Flow.ToString());
        //    //list.Add(_deviceDyn.Dyn.Signal.ToString());
        //    //OnDeviceObjectChanged(list.ToArray());
        //}

        public override void UnknownIO()
        {
            OnDeviceRuningLog("未知通讯接口");
        }

        public override void CommunicateStateChanged(ServerSuperIO.Communicate.CommunicateState comState)
        {
            OnDeviceRuningLog("通讯状态改变");
        }

        public override void ChannelStateChanged(ServerSuperIO.Communicate.ChannelState channelState)
        {
            OnDeviceRuningLog("通道状态改变");
        }

        public override void Exit()
        {
            OnDeviceRuningLog("退出设备");
        }

        public override void Delete()
        {
            OnDeviceRuningLog("删除设备");
        }

        public override object GetObject()
        {
            throw new NotImplementedException();
        }

        //public override string GetAlertState()
        //{
        //    return "";
        //}

        public override void ShowContextMenu()
        {
            throw new NotImplementedException();
        }

        public override IDeviceDynamic DeviceDynamic
        {
            get { return _deviceDyn; }
        }

        public override IDeviceParameter DeviceParameter
        {
            get { return _devicePara; }
        }

        public override IProtocolDriver Protocol {
            get { return _protocol; }
        }

        public override DeviceType DeviceType
        {
            get { return DeviceType.Common; }
        }

        public override string ModelNumber
        {
            get { return "serversuperio"; }
        }


        public override IDeviceConnectorCallbackResult RunDeviceConnector(IFromDevice fromDevice, IDeviceToDevice toDevice,
            AsyncDeviceConnectorCallback asyncCallback)
        {
            throw new NotImplementedException();
        }

        public override void DeviceConnectorCallback(object obj)
        {
            throw new NotImplementedException();
        }

        public override void DeviceConnectorCallbackError(Exception ex)
        {
            throw new NotImplementedException();
        }

        public override IServiceConnectorCallbackResult RunServiceConnector(IFromService fromService, IServiceToDevice toDevice,
            AsyncServiceConnectorCallback asyncService)
        {
            throw new NotImplementedException();
        }
    }
}
