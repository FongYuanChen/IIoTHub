using EZNCAUTLib;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Requests;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Responses;
using IIoTHub.Infrastructure.DeviceDriverHost.FCSB1224W000.x86.Utilities;

namespace IIoTHub.Infrastructure.DeviceDriverHost.FCSB1224W000.x86.Hosts
{
    /// <summary>
    /// FCSB1224W000 運行核心
    /// </summary>
    public class FCSB1224W000Runtime
    {
        private readonly FCSB1224W000ConnectionPool _connectionPool = new();

        /// <summary>
        /// 取得運行狀態資訊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FCSB1224W000StateInfoResponse GetStateInfo(FCSB1224W000StateInfoRequest request)
        {
            return Execute(
                request,
                communication => FCSB1224W000StateInfoResponse.Success(request.RequestId, FCSB1224W000Helper.GetStateInfo(communication)),
                ex => FCSB1224W000StateInfoResponse.FromException(request.RequestId, ex));
        }

        /// <summary>
        /// 取得警報資訊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FCSB1224W000AlarmInfoResponse GetAlarmInfo(FCSB1224W000AlarmInfoRequest request)
        {
            return Execute(
                request,
                communication => FCSB1224W000AlarmInfoResponse.Success(request.RequestId, FCSB1224W000Helper.GetAlarmInfo(communication)),
                ex => FCSB1224W000AlarmInfoResponse.FromException(request.RequestId, ex));
        }

        /// <summary>
        /// 取得程式資訊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FCSB1224W000ProgramInfoResponse GetProgramInfo(FCSB1224W000ProgramInfoRequest request)
        {
            return Execute(
                request,
                communication => FCSB1224W000ProgramInfoResponse.Success(request.RequestId, FCSB1224W000Helper.GetProgramInfo(communication)),
                ex => FCSB1224W000ProgramInfoResponse.FromException(request.RequestId, ex));
        }

        /// <summary>
        /// 取得主軸資訊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FCSB1224W000SpindleInfoResponse GetSpindleInfo(FCSB1224W000SpindleInfoRequest request)
        {
            return Execute(
                request,
                communication => FCSB1224W000SpindleInfoResponse.Success(request.RequestId, FCSB1224W000Helper.GetSpindleInfo(communication)),
                ex => FCSB1224W000SpindleInfoResponse.FromException(request.RequestId, ex));
        }

        /// <summary>
        /// 取得位置資訊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FCSB1224W000PositionInfoResponse GetPositionInfo(FCSB1224W000PositionInfoRequest request)
        {
            return Execute(
                request,
                communication => FCSB1224W000PositionInfoResponse.Success(request.RequestId, FCSB1224W000Helper.GetPositionInfo(communication)),
                ex => FCSB1224W000PositionInfoResponse.FromException(request.RequestId, ex));
        }

        /// <summary>
        /// 執行請求的共用方法
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="executeWithCommunication"></param>
        /// <param name="createFailureResponse"></param>
        /// <returns></returns>
        private TResponse Execute<TRequest, TResponse>(TRequest request,
                                                       Func<DispEZNcCommunication, TResponse> executeWithCommunication,
                                                       Func<Exception, TResponse> createFailureResponse) where TRequest : FCSB1224W000BaseInfoRequest
                                                                                                         where TResponse : FCSB1224W000BaseInfoResponse
        {
            try
            {
                var connection = _connectionPool.GetOrCreate(request.SystemType, request.IpAddress, request.Port, request.Timeout);
                lock (connection.SyncRoot)
                {
                    connection.EnsureConnected();
                    return executeWithCommunication(connection.Communication);
                }
            }
            catch (Exception ex)
            {
                return createFailureResponse(ex);
            }
        }
    }
}
