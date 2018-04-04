using System;
using System.Threading.Tasks;
using Solution.Device.Core;

namespace Solution.Device.OpcUaDevice
{
    public class OpcUaDevice : IDevice
    {
        /// <summary>
        /// 通知事件,设备状态、数据异常等都通过此事件对外发布
        /// </summary>
        public event DeviceNotificationEventHandler Notification;


        public OpcUaDevice()
        {


        }

        /// <summary>
        /// 连接/打开设备
        /// </summary>
        /// <param name="param">连接/打开设备</param>
        /// <returns>连接/打开设备的结果</returns>
        public Task<bool> Connect(object param)
        {
            Notification(this, new DeviceEventArgs(DateTime.Now));
            throw new NotImplementedException();

        }

        /// <summary>
        /// 断开/关闭设备
        /// </summary>
        /// <returns>断开/关闭设备的结果</returns>
        public Task<bool> DisConnect()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="data">写入的数据</param>
        /// <returns>写入的结果</returns>
        public Task<object> Write(object data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="data">写入的数据</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns>写入的结果</returns>
        public Task<object> Write(object data, uint timeOut)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 读
        /// </summary>
        /// <param name="data">要读取的数据</param>
        /// <returns>读取的结果</returns>
        public Task<object> Read(object data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 读
        /// </summary>
        /// <param name="data">要读取的数据</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns>读取的结果</returns>
        public Task<object> Read(object data, uint timeOut)
        {
            throw new NotImplementedException();
        }
    }
}
