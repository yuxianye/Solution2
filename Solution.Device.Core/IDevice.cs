using System;
using System.Threading.Tasks;

namespace Solution.Device.Core
{
    /// <summary>
    /// 用于设备通知的代理
    /// </summary>
    /// <param name="device"></param>
    /// <param name="deviceEventArgs"></param>
    public delegate void DeviceNotificationEventHandler(IDevice device, DeviceEventArgs deviceEventArgs);

    /// <summary>
    /// 设备接口
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// 通知事件,设备状态、数据异常等都通过此事件对外发布
        /// </summary>
        event DeviceNotificationEventHandler Notification;

        /// <summary>
        /// 连接/打开设备
        /// </summary>
        /// <param name="param">连接/打开设备</param>
        /// <returns>连接/打开设备的结果</returns>
        Task<bool> Connect(object param);

        /// <summary>
        /// 断开/关闭设备
        /// </summary>
        /// <returns>断开/关闭设备的结果</returns>
        Task<bool> DisConnect();

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="data">写入的数据</param>
        /// <returns>写入的结果</returns>
        Task<object> Write(object data);

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="data">写入的数据</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns>写入的结果</returns>
        Task<object> Write(object data, uint timeOut);

        /// <summary>
        /// 读
        /// </summary>
        /// <param name="data">要读取的数据</param>
        /// <returns>读取的结果</returns>
        Task<object> Read(object data);

        /// <summary>
        /// 读
        /// </summary>
        /// <param name="data">要读取的数据</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns>读取的结果</returns>
        Task<object> Read(object data, uint timeOut);
    }
}
