using IIoTHub.Infrastructure.DeviceDriverHosts.Models;
using System.Diagnostics;

namespace IIoTHub.Infrastructure.DeviceDriverHosts
{
    /// <summary>
    /// 設備驅動器宿主進程
    /// </summary>
    public class DeviceDriverHostProcess : IDisposable
    {
        private readonly DeviceDriverHostDescriptor _descriptor;
        private readonly object _syncRoot = new();

        private Process _process;

        public DeviceDriverHostProcess(DeviceDriverHostDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        /// <summary>
        /// 判斷宿主進程是否正在執行
        /// </summary>
        private bool IsRunning
            => _process is { HasExited: false };

        /// <summary>
        /// 確保宿主進程正在執行。
        /// 若進程尚未啟動，會啟動它。
        /// </summary>
        public void EnsureRunning()
        {
            lock (_syncRoot)
            {
                if (IsRunning)
                    return;

                StartInternal();
            }
        }

        /// <summary>
        /// 啟動宿主進程
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        private void StartInternal()
        {
            if (!File.Exists(_descriptor.ExeFilePath))
                throw new FileNotFoundException($"找不到 {_descriptor.ExeFilePath}");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _descriptor.ExeFilePath,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            process.Exited += OnProcessExited;
            process.Start();

            _process = process;
        }

        /// <summary>
        /// 進程退出事件處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnProcessExited(object sender, EventArgs e)
        {
            lock (_syncRoot)
            {
                CleanupProcess();
            }
        }

        /// <summary>
        /// 清理 Process 物件
        /// </summary>
        private void CleanupProcess()
        {
            if (_process == null)
                return;

            _process.Exited -= OnProcessExited;
            _process.Dispose();
            _process = null;
        }

        /// <summary>
        /// 停止宿主進程並釋放資源
        /// </summary>
        public void Dispose()
        {
            lock (_syncRoot)
            {
                if (!IsRunning)
                    return;

                try
                {
                    _process.Kill(entireProcessTree: true);
                    _process.WaitForExit();
                }
                catch (Exception)
                {
                }
                finally
                {
                    CleanupProcess();
                }
            }
        }
    }
}
