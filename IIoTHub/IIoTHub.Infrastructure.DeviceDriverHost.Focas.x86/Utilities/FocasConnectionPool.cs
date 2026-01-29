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

        public FocasConnection(string ip,
                               int port,
                               int timeout)
        {
            _ip = ip;
            _port = port;
            _timeout = timeout;
        }

        /// <summary>
        /// 提供外部同步用鎖物件（例如批次操作）
        /// </summary>
        public object SyncRoot => _syncRoot;

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

                Connect();
            }
        }

        /// <summary>
        /// 檢查當前連線句柄是否有效
        /// </summary>
        /// <returns></returns>
        private bool IsHandleValid()
        {
            if (_handle == 0)
                return false;

            try
            {
                FocasHelper.EnsureOk(Focas1.cnc_sysinfo(_handle, new Focas1.ODBSYS()));
                return true;
            }
            catch
            {
                Disconnect();
                return false;
            }
        }

        /// <summary>
        /// 建立連線句柄並連線
        /// </summary>
        private void Connect()
        {
            Disconnect();

            FocasHelper.EnsureOk(Focas1.cnc_allclibhndl3(_ip, (ushort)_port, _timeout, out _handle));
        }

        /// <summary>
        /// 斷線釋放資源
        /// </summary>
        private void Disconnect()
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

        /// <summary>
        /// 釋放連線資源
        /// </summary>
        public void Dispose()
        {
            lock (_syncRoot)
            {
                Disconnect();
            }
        }
    }
}
