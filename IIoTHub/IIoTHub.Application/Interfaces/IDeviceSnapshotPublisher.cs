using IIoTHub.Domain.Models.DeviceSnapshots;

namespace IIoTHub.Application.Interfaces
{
    /// <summary>
    /// 設備快照發佈器介面
    /// </summary>
    public interface IDeviceSnapshotPublisher
    {
        /// <summary>
        /// 發佈指定設備的快照更新
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="snapshot"></param>
        void Publish<TSnapshot>(Guid deviceId, TSnapshot snapshot) where TSnapshot : DeviceSnapshot;

        /// <summary>
        /// 訂閱指定設備的快照更新
        /// </summary>
        /// <typeparam name="TSnapshot"></typeparam>
        /// <param name="deviceId"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        IDisposable Subscribe<TSnapshot>(Guid deviceId, Action<TSnapshot> handler) where TSnapshot : DeviceSnapshot;
    }
}
