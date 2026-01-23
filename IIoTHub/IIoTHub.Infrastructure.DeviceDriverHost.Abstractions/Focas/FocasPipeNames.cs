namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas
{
    /// <summary>
    /// FOCAS 相關的命名管道名稱常數集合
    /// </summary>
    public static class FocasPipeNames
    {
        /// <summary>
        /// 32 位元 FOCAS 驅動器宿主的命名管道名稱。
        /// 用於與 x86 架構的 DeviceDriverHost 進程進行通訊。
        /// </summary>
        public const string DriverHostX86 =
            "IIoTHub.Infrastructure.DeviceDriverHost.Focas.x86";
    }
}
