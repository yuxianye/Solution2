using System;

namespace Solution.Device.Core
{
    /// <summary>
    /// 设备事件参数
    /// </summary>
    public class DeviceEventArgs : EventArgs
    {
        /// <summary>
        /// 设备事件参数的构造函数
        /// </summary>
        /// <param name="data">事件的数据</param>
        public DeviceEventArgs(object data)
        {
            this.Data = data;
        }

        /// <summary>
        /// 事件的数据
        /// </summary>
        public object Data { get; set; }
    }

}
