using IIoTHub.Domain.Models.DeviceRecords;

namespace IIoTHub.Domain.Interfaces.Repositories
{
    /// <summary>
    /// 提供新增、更新、刪除、查詢等操作的設備運轉紀錄Repository介面
    /// </summary>
    public interface IDeviceRuntimeRepository
    {
        /// <summary>
        /// 新增一筆設備運轉紀錄
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        Task AddAsync(DeviceRuntimeRecord record);

        /// <summary>
        /// 更新指定的設備運轉紀錄
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        Task UpdateAsync(DeviceRuntimeRecord record);

        /// <summary>
        /// 取得指定設備在特定時間區間內的運轉紀錄
        /// </summary>
        Task<IEnumerable<DeviceRuntimeRecord>> GetRecordsAsync(Guid deviceId, DateTime from, DateTime to);

        /// <summary>
        /// 取得指定設備最新的一筆運轉紀錄
        /// </summary>
        Task<DeviceRuntimeRecord> GetLatestRecordAsync(Guid deviceId);
    }
}
