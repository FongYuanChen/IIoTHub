using IIoTHub.Application.Enums;
using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces.Repositories;
using IIoTHub.Domain.Models.DeviceMonitor;
using IIoTHub.Domain.Models.DeviceSettings;
using IIoTHub.Domain.Models.DeviceSnapshots;
using System.Collections.Concurrent;

namespace IIoTHub.Application.Services
{
    public class DeviceSnapshotMonitorService : IDeviceSnapshotMonitorService
    {
        private readonly IDeviceSnapshotMonitorStatusRepository _deviceSnapshotMonitorStatusRepository;
        private readonly IDeviceSettingRepository _deviceSettingRepository;
        private readonly IDeviceRuntimeStatisticsService _deviceRuntimeStatisticsService;
        private readonly IDeviceSnapshotService _deviceSnapshotService;
        private readonly IDeviceSnapshotPublisher _deviceSnapshotPublisher;

        private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _monitorTasks = new();

        public DeviceSnapshotMonitorService(IDeviceSnapshotMonitorStatusRepository deviceSnapshotMonitorStatusRepository,
                                            IDeviceSettingRepository deviceSettingRepository,
                                            IDeviceRuntimeStatisticsService deviceRuntimeStatisticsService,
                                            IDeviceSnapshotService deviceSnapshotService,
                                            IDeviceSnapshotPublisher deviceSnapshotPublisher)
        {
            _deviceSnapshotMonitorStatusRepository = deviceSnapshotMonitorStatusRepository;
            _deviceSettingRepository = deviceSettingRepository;
            _deviceRuntimeStatisticsService = deviceRuntimeStatisticsService;
            _deviceSnapshotService = deviceSnapshotService;
            _deviceSnapshotPublisher = deviceSnapshotPublisher;
        }

        /// <summary>
        /// 啟動指定設備的監控
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task StartMonitorAsync(Guid deviceId)
        {
            // 更新監控狀態 (持久化)
            var status = await _deviceSnapshotMonitorStatusRepository.GetByIdAsync(deviceId) ?? new DeviceMonitorStatus(deviceId);
            status.IsMonitoring = true;
            await _deviceSnapshotMonitorStatusRepository.UpdateAsync(status);

            // 啟動監控
            if (_monitorTasks.TryAdd(deviceId, new CancellationTokenSource()))
            {
                var cts = _monitorTasks[deviceId];
                _ = Task.Run(() => MonitorLoopAsync(deviceId, cts.Token));
            }
        }

        /// <summary>
        /// 設備監控
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task MonitorLoopAsync(Guid deviceId, CancellationToken token)
        {
            // 啟動監控前先標記離線
            await _deviceRuntimeStatisticsService.OnRuntimeStatusChangedAsync(deviceId, DeviceRunStatus.Offline, DateTime.Now);

            // 啟動監控
            DeviceSetting deviceSetting;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    deviceSetting = await _deviceSettingRepository.GetByIdAsync(deviceId);
                    switch (deviceSetting.CategoryType)
                    {
                        case DeviceCategoryType.Machine:
                            var machineSnapshot = await _deviceSnapshotService.GetMachineSnapshotAsync(deviceId);
                            _deviceSnapshotPublisher.Publish(deviceId, machineSnapshot);
                            await _deviceRuntimeStatisticsService.OnRuntimeStatusChangedAsync(deviceId, machineSnapshot.RunStatus, machineSnapshot.Timestamp);
                            break;
                        case DeviceCategoryType.Magazine:
                            var magazineSnapshot = await _deviceSnapshotService.GetMagazineSnapshotAsync(deviceId);
                            _deviceSnapshotPublisher.Publish(deviceId, magazineSnapshot);
                            await _deviceRuntimeStatisticsService.OnRuntimeStatusChangedAsync(deviceId, magazineSnapshot.RunStatus, magazineSnapshot.Timestamp);
                            break;
                        case DeviceCategoryType.Robot:
                            var robotSnapshot = await _deviceSnapshotService.GetRobotSnapshotAsync(deviceId);
                            _deviceSnapshotPublisher.Publish(deviceId, robotSnapshot);
                            await _deviceRuntimeStatisticsService.OnRuntimeStatusChangedAsync(deviceId, robotSnapshot.RunStatus, robotSnapshot.Timestamp);
                            break;
                    }
                }
                catch (OperationCanceledException)
                {
                    // 正常停止
                    // 標記離線，並通知所有訂閱者
                    deviceSetting = await _deviceSettingRepository.GetByIdAsync(deviceId);
                    switch (deviceSetting.CategoryType)
                    {
                        case DeviceCategoryType.Machine:
                            var machineSnapshot = MachineSnapshot.OfflineSnapshot(deviceId);
                            _deviceSnapshotPublisher.Publish(deviceId, machineSnapshot);
                            await _deviceRuntimeStatisticsService.OnRuntimeStatusChangedAsync(deviceId, machineSnapshot.RunStatus, machineSnapshot.Timestamp);
                            break;
                        case DeviceCategoryType.Magazine:
                            var magazineSnapshot = MagazineSnapshot.OfflineSnapshot(deviceId);
                            _deviceSnapshotPublisher.Publish(deviceId, magazineSnapshot);
                            await _deviceRuntimeStatisticsService.OnRuntimeStatusChangedAsync(deviceId, magazineSnapshot.RunStatus, magazineSnapshot.Timestamp);
                            break;
                        case DeviceCategoryType.Robot:
                            var robotSnapshot = RobotSnapshot.OfflineSnapshot(deviceId);
                            _deviceSnapshotPublisher.Publish(deviceId, robotSnapshot);
                            await _deviceRuntimeStatisticsService.OnRuntimeStatusChangedAsync(deviceId, robotSnapshot.RunStatus, robotSnapshot.Timestamp);
                            break;
                    }
                    break;
                }
                catch (Exception)
                {
                }

                // 關鍵：跑完才等 1 秒
                await Task.Delay(TimeSpan.FromSeconds(1), token);
            }
        }

        /// <summary>
        /// 停止指定設備的監控
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public async Task StopMonitorAsync(Guid deviceId, StopMonitorReason reason)
        {
            // 更新監控狀態 (持久化)
            switch (reason)
            {
                case StopMonitorReason.Temporary:
                    var status = await _deviceSnapshotMonitorStatusRepository.GetByIdAsync(deviceId) ?? new DeviceMonitorStatus(deviceId);
                    status.IsMonitoring = false;
                    await _deviceSnapshotMonitorStatusRepository.UpdateAsync(status);
                    break;
                case StopMonitorReason.Removed:
                    await _deviceSnapshotMonitorStatusRepository.DeleteAsync(deviceId);
                    break;
                default:
                    break;
            }

            // 關閉監控
            if (_monitorTasks.TryRemove(deviceId, out var cts))
            {
                cts.Cancel();
            }
        }

        /// <summary>
        /// 檢查指定設備是否正在監控中
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<bool> IsMonitoringAsync(Guid deviceId)
            => (await _deviceSnapshotMonitorStatusRepository.GetByIdAsync(deviceId))?.IsMonitoring ?? false;
    }
}
