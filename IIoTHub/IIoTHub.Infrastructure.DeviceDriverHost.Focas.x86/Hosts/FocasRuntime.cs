using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Requests;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Responses;
using IIoTHub.Infrastructure.DeviceDriverHost.Focas.x86.Utilities;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Focas.x86.Hosts
{
    /// <summary>
    /// FOCAS 運行核心
    /// </summary>
    public class FocasRuntime
    {
        private readonly FocasConnectionPool _connectionPool = new();

        /// <summary>
        /// 取得運行狀態資訊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FocasStateInfoResponse GetStateInfo(FocasStateInfoRequest request)
        {
            return Execute(
                request,
                handle => FocasStateInfoResponse.Success(request.RequestId, FocasHelper.GetStateInfo(handle)),
                ex => FocasStateInfoResponse.FromException(request.RequestId, ex));
        }

        /// <summary>
        /// 取得警報資訊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FocasAlarmInfoResponse GetAlarmInfo(FocasAlarmInfoRequest request)
        {
            return Execute(
                request,
                handle => FocasAlarmInfoResponse.Success(request.RequestId, FocasHelper.GetAlarmInfo(handle)),
                ex => FocasAlarmInfoResponse.FromException(request.RequestId, ex));
        }

        /// <summary>
        /// 取得程式資訊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FocasProgramInfoResponse GetProgramInfo(FocasProgramInfoRequest request)
        {
            return Execute(
                request,
                handle => FocasProgramInfoResponse.Success(request.RequestId, FocasHelper.GetProgramInfo(handle)),
                ex => FocasProgramInfoResponse.FromException(request.RequestId, ex));
        }

        /// <summary>
        /// 取得主軸資訊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FocasSpindleInfoResponse GetSpindleInfo(FocasSpindleInfoRequest request)
        {
            return Execute(
                request,
                handle => FocasSpindleInfoResponse.Success(request.RequestId, FocasHelper.GetSpindleInfo(handle)),
                ex => FocasSpindleInfoResponse.FromException(request.RequestId, ex));
        }

        /// <summary>
        /// 取得位置資訊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FocasPositionInfoResponse GetPositionInfo(FocasPositionInfoRequest request)
        {
            return Execute(
                request,
                handle => FocasPositionInfoResponse.Success(request.RequestId, FocasHelper.GetPositionInfo(handle)),
                ex => FocasPositionInfoResponse.FromException(request.RequestId, ex));
        }

        /// <summary>
        /// 執行請求的共用方法
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="executeWithHandle"></param>
        /// <param name="createFailureResponse"></param>
        /// <returns></returns>
        private TResponse Execute<TRequest, TResponse>(TRequest request,
                                                       Func<ushort, TResponse> executeWithHandle,
                                                       Func<Exception, TResponse> createFailureResponse) where TRequest : FocasBaseInfoRequest
                                                                                                         where TResponse : FocasBaseInfoResponse
        {
            try
            {
                var connection = _connectionPool.GetOrCreate(request.IpAddress, request.Port, request.Timeout);
                lock (connection)
                {
                    connection.EnsureConnected();
                    return executeWithHandle(connection.Handle);
                }
            }
            catch (Exception ex)
            {
                return createFailureResponse(ex);
            }
        }
    }
}
