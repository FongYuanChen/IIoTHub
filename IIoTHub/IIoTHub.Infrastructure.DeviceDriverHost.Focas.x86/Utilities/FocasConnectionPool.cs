using System.Collections.Concurrent;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Focas.x86.Utilities
{
    /// <summary>
    /// FOCAS 連線池
    /// </summary>
    public class FocasConnectionPool : IDisposable
    {
        private readonly ConcurrentDictionary<FocasConnectionKey, FocasConnection> _connections = new();

        /// <summary>
        /// 根據指定的 IP、Port 與 Timeout 取得現有連線，若不存在則建立新的連線
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public FocasConnection GetOrCreate(string ip, int port, int timeout)
        {
            var key = new FocasConnectionKey(ip, port, timeout);
            return _connections.GetOrAdd(key, _ => new FocasConnection(ip, port, timeout));
        }

        /// <summary>
        /// 釋放所有連線並清空連線池
        /// </summary>
        public void Dispose()
        {
            foreach (var connection in _connections.Values)
            {
                connection.Dispose();
            }
            _connections.Clear();
        }

        /// <summary>
        /// FOCAS 連線的唯一鍵
        /// </summary>
        /// <param name="Ip"></param>
        /// <param name="Port"></param>
        /// <param name="Timeout"></param>
        internal readonly record struct FocasConnectionKey(string Ip, int Port, int Timeout);
    }

    /// <summary>
    /// FOCAS 連線
    /// </summary>
    public class FocasConnection
    {
        private readonly string _ip;
        private readonly int _port;
        private readonly int _timeout;

        private ushort _handle;
        private readonly object _syncRoot = new();

        private DateTime _nextReconnectAt = DateTime.MinValue;
        private TimeSpan _nextRetryDelay = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan MaxRetryDelay = TimeSpan.FromSeconds(30);

        public FocasConnection(string ip,
                               int port,
                               int timeout)
        {
            _ip = ip;
            _port = port;
            _timeout = timeout;
        }

        /// <summary>
        /// FOCAS 連線句柄
        /// </summary>
        public ushort Handle
        {
            get
            {
                lock (_syncRoot)
                {
                    return _handle;
                }
            }
        }

        /// <summary>
        /// 確保連線有效，若未連線或斷線則嘗試重新連線
        /// </summary>
        /// <returns></returns>
        public void EnsureConnected()
        {
            lock (_syncRoot)
            {
                if (IsHandleValid())
                    return;

                if (!IsReconnectAllowed())
                    throw new FocasException(Focas1.EW_BUSY);

                TryReconnect();
            }
        }

        /// <summary>
        /// 檢查當前 handle 是否有效
        /// </summary>
        /// <returns></returns>
        private bool IsHandleValid()
        {
            if (_handle == 0)
                return false;

            // 檢查 handle 是否仍有效
            try
            {
                var result = Focas1.cnc_sysinfo(_handle, new Focas1.ODBSYS());
                if (result == Focas1.EW_OK)
                    return true;
            }
            catch
            {
            }

            // handle 已失效，釋放
            try
            {
                Focas1.cnc_freelibhndl(_handle);
            } 
            catch
            {
            }

            _handle = 0;
            return false;
        }

        /// <summary>
        /// 檢查是否允許重新連線
        /// </summary>
        /// <returns></returns>
        private bool IsReconnectAllowed()
        {
            return DateTime.Now >= _nextReconnectAt;
        }

        /// <summary>
        /// 嘗試重新建立連線
        /// </summary>
        /// <returns></returns>
        private void TryReconnect()
        {
            var result = Focas1.cnc_allclibhndl3(_ip, (ushort)_port, _timeout, out _handle);
            if (result != Focas1.EW_OK)
            {
                _handle = 0;
                ScheduleNextRetry();
                throw new FocasException(result);
            }

            ResetNextRetry();
        }

        /// <summary>
        /// 重置重連排程與延遲時間
        /// </summary>
        private void ResetNextRetry()
        {
            _nextReconnectAt = DateTime.MinValue;
            _nextRetryDelay = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// 設定下次重連排程與延遲時間
        /// </summary>
        private void ScheduleNextRetry()
        {
            _nextReconnectAt = DateTime.Now + _nextRetryDelay;

            // 指數退避
            var next = TimeSpan.FromSeconds(_nextRetryDelay.TotalSeconds * 2);
            _nextRetryDelay = next <= MaxRetryDelay ? next : MaxRetryDelay;
        }

        /// <summary>
        /// 釋放連線資源
        /// </summary>
        public void Dispose()
        {
            lock (_syncRoot)
            {
                if (_handle != 0)
                {
                    try
                    {
                        Focas1.cnc_freelibhndl(_handle);
                        _handle = 0;
                    }
                    catch
                    {
                    }
                }  
            }
        }
    }
}
