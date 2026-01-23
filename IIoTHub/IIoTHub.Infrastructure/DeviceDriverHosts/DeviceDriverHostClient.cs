using Newtonsoft.Json;
using System.IO.Pipes;

namespace IIoTHub.Infrastructure.DeviceDriverHosts
{
    /// <summary>
    /// 設備驅動器宿主客戶端
    /// </summary>
    public class DeviceDriverHostClient
    {
        /// <summary>
        /// 透過命名管道發送請求給設備驅動宿主，並等待回應
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="pipeName"></param>
        /// <param name="request"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static TResponse Send<TRequest, TResponse>(string pipeName, TRequest request, int timeoutSeconds = 10)
        {
            using var client = new NamedPipeClientStream(
                serverName: ".",
                pipeName: pipeName,
                direction: PipeDirection.InOut,
                options: PipeOptions.Asynchronous);

            client.Connect(timeoutSeconds * 1000);

            using var reader = new StreamReader(client);
            using var writer = new StreamWriter(client) { AutoFlush = true };

            writer.WriteLine(JsonConvert.SerializeObject(request));

            return JsonConvert.DeserializeObject<TResponse>(reader.ReadLine());
        }
    }
}
