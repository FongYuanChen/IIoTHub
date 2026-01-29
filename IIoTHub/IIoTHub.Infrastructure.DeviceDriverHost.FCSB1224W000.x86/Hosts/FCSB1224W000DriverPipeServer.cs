using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Enums;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Requests;
using Newtonsoft.Json;
using System.IO.Pipes;

namespace IIoTHub.Infrastructure.DeviceDriverHost.FCSB1224W000.x86.Hosts
{
    /// <summary>
    /// FCSB1224W000 驅動器命名管道伺服器
    /// </summary>
    public class FCSB1224W000DriverPipeServer
    {
        private readonly FCSB1224W000Runtime _runtime = new();

        /// <summary>
        /// 啟動 Pipe Server
        /// 
        /// 說明：
        /// - 持續等待 Client 連線
        /// - 每個 Client 建立一個獨立的 PipeServerStream
        /// - 每個 Client 交由獨立 Task 處理
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task RunAsync(CancellationToken token)
        {
            Console.WriteLine("【PipeServer】Mitsubishi FCSB1224W000 Driver Host 已啟動，等待 Client 連線中...");

            while (!token.IsCancellationRequested)
            {
                var pipeServer = new NamedPipeServerStream(
                    FCSB1224W000PipeNames.DriverHostX86,
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Message,
                    PipeOptions.Asynchronous);

                Console.WriteLine("【PipeServer】等待新的 Client 連線...");

                await pipeServer.WaitForConnectionAsync(token);

                Console.WriteLine("【PipeServer】Client 已連線，建立處理 Task");

                _ = Task.Run(() => HandleClientAsync(pipeServer), token);
            }
        }

        /// <summary>
        /// 處理單一 Client 的請求生命週期
        /// 
        /// 流程：
        /// 1. 讀取 Client 傳來的 JSON Request（一行一個 Request）
        /// 2. 反序列化成 Base Request 以取得 CommandType
        /// 3. Dispatch 到對應的 Runtime 方法
        /// 4. 將 Response 以 JSON 回傳給 Client
        /// 
        /// 結束條件：
        /// - Client 主動關閉 Pipe
        /// - 發生例外（Client 中斷、格式錯誤等）
        /// </summary>
        /// <param name="pipeServer"></param>
        /// <returns></returns>
        private async Task HandleClientAsync(NamedPipeServerStream pipeServer)
        {
            Console.WriteLine("【PipeServer】開始處理 Client 請求");

            try
            {
                await using (pipeServer)
                using (StreamReader reader = new StreamReader(pipeServer))
                using (StreamWriter writer = new StreamWriter(pipeServer) { AutoFlush = true })
                {
                    while (await reader.ReadLineAsync() is string requestJson)
                    {
                        Console.WriteLine($"【PipeServer】收到 Request：{requestJson}");

                        var request = JsonConvert.DeserializeObject<FCSB1224W000BaseInfoRequest>(requestJson);
                        var response = Dispatch(request.CommandType, requestJson);
                        var responseJson = JsonConvert.SerializeObject(response);
                        await writer.WriteLineAsync(responseJson);

                        Console.WriteLine($"【PipeServer】回傳 Response：{responseJson}");
                    }

                    Console.WriteLine("【PipeServer】Client 已中斷連線");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"【PipeServer】處理 Client 時發生例外：{ex}");
            }
        }

        /// <summary>
        /// 依據 CommandType 派送對應的 Runtime 方法
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="requestJson"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private object Dispatch(FCSB1224W000CommandType commandType, string requestJson)
        {
            return commandType switch
            {
                FCSB1224W000CommandType.GetStateInfo => _runtime.GetStateInfo(JsonConvert.DeserializeObject<FCSB1224W000StateInfoRequest>(requestJson)),
                FCSB1224W000CommandType.GetAlarmInfo => _runtime.GetAlarmInfo(JsonConvert.DeserializeObject<FCSB1224W000AlarmInfoRequest>(requestJson)),
                FCSB1224W000CommandType.GetProgramInfo => _runtime.GetProgramInfo(JsonConvert.DeserializeObject<FCSB1224W000ProgramInfoRequest>(requestJson)),
                FCSB1224W000CommandType.GetSpindleInfo => _runtime.GetSpindleInfo(JsonConvert.DeserializeObject<FCSB1224W000SpindleInfoRequest>(requestJson)),
                FCSB1224W000CommandType.GetPositionInfo => _runtime.GetPositionInfo(JsonConvert.DeserializeObject<FCSB1224W000PositionInfoRequest>(requestJson)),
                _ => throw new NotImplementedException($"未支援的 CommandType：{commandType}")
            };
        }
    }
}
