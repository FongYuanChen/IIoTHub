using EZNCAUTLib;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Enums;
using System.Collections.Concurrent;

namespace IIoTHub.Infrastructure.DeviceDriverHost.FCSB1224W000.x86.Utilities
{
    /// <summary>
    /// FCSB1224W000 連線池
    /// </summary>
    public class FCSB1224W000ConnectionPool
    {
        private readonly ConcurrentDictionary<FCSB1224W000ConnectionKey, FCSB1224W000Connection> _connections = new();
        private int _currentIndex;

        /// <summary>
        /// 根據指定的 SystemType、IP、Port 與 Timeout 取得現有連線，若不存在則建立新的連線
        /// </summary>
        /// <param name="systemType"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public FCSB1224W000Connection GetOrCreate(FCSB1224W000SystemType systemType, string ip, int port, int timeout)
        {
            var key = new FCSB1224W000ConnectionKey(systemType, ip, port, timeout);
            return _connections.GetOrAdd(
                key,
                _ =>
                {
                    _currentIndex++;
                    return new FCSB1224W000Connection(systemType, _currentIndex, ip, port, timeout);
                });
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
            _currentIndex = 0;
        }

        /// <summary>
        /// FCSB1224W000 連線的唯一鍵
        /// </summary>
        /// <param name="SystemType"></param>
        /// <param name="Ip"></param>
        /// <param name="Port"></param>
        /// <param name="Timeout"></param>
        internal readonly record struct FCSB1224W000ConnectionKey(FCSB1224W000SystemType SystemType, string Ip, int Port, int Timeout);
    }

    /// <summary>
    /// FCSB1224W000 連線
    /// </summary>
    public class FCSB1224W000Connection
    {
        private readonly FCSB1224W000SystemType _systemType;
        private readonly int _index;
        private readonly string _ip;
        private readonly int _port;
        private readonly int _timeout;

        private DispEZNcCommunication _communication;
        private readonly object _syncRoot = new();

        public FCSB1224W000Connection(FCSB1224W000SystemType systemType,
                                      int index,
                                      string ip,
                                      int port,
                                      int timeout)
        {
            _systemType = systemType;
            _index = index;
            _ip = ip;
            _port = port;
            _timeout = timeout;
        }

        /// <summary>
        /// 提供外部同步用鎖物件（例如批次操作）
        /// </summary>
        public object SyncRoot => _syncRoot;

        /// <summary>
        /// FCSB1224W000 通訊物件
        /// </summary>
        public DispEZNcCommunication Communication
        {
            get
            {
                lock (_syncRoot)
                {
                    return _communication;
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
                if (IsCommunicationValid())
                    return;

                Connect();
            }
        }

        /// <summary>
        /// 檢查當前通訊物件是否有效
        /// </summary>
        /// <returns></returns>
        private bool IsCommunicationValid()
        {
            if (_communication == null)
                return false;

            try
            {
                FCSB1224W000Helper.EnsureOk(_communication.System_GetSerialNo(out _));
                return true;
            }
            catch
            {
                Disconnect();
                return false;
            }
        }

        /// <summary>
        /// 建立通訊物件並連線
        /// </summary>
        private void Connect()
        {
            Disconnect();

            _communication = new DispEZNcCommunication();
            FCSB1224W000Helper.EnsureOk(_communication.SetTCPIPProtocol(_ip, _port));
            var systemType = _systemType switch
            {
                FCSB1224W000SystemType.M700L => FCSB1224W000Parameter.EZNC_SYS_MELDAS700L,
                FCSB1224W000SystemType.M700M => FCSB1224W000Parameter.EZNC_SYS_MELDAS700M,
                FCSB1224W000SystemType.M800L => FCSB1224W000Parameter.EZNC_SYS_MELDAS800L,
                FCSB1224W000SystemType.M800M => FCSB1224W000Parameter.EZNC_SYS_MELDAS800M,
                _ => default
            };
            var multiThreadSystemType = systemType | FCSB1224W000Parameter.EZNC_SYS_MULTI;
            FCSB1224W000Helper.EnsureOk(_communication.Open3(multiThreadSystemType, _index, _timeout * 10, "EZNC_LOCALHOST")); // 逾時單位: 100ms
        }

        /// <summary>
        /// 斷線釋放資源
        /// </summary>
        private void Disconnect()
        {
            try
            {
                _communication?.Close2(FCSB1224W000Parameter.EZNC_RESET_SIMPLE);
                _communication = null;
            }
            catch
            {
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
