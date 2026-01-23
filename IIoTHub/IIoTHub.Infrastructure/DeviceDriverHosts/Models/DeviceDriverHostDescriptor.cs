namespace IIoTHub.Infrastructure.DeviceDriverHosts.Models
{
    /// <summary>
    /// 設備驅動器宿主描述物件
    /// </summary>
    public class DeviceDriverHostDescriptor
    {
        public DeviceDriverHostDescriptor(string name,
                                          string exeFilePath)
        {
            Name = name;
            ExeFilePath = exeFilePath;
        }

        /// <summary>
        /// 設備驅動器宿主名稱
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 設備驅動器宿主可執行檔完整路徑
        /// </summary>
        public string ExeFilePath { get; }
    }
}
