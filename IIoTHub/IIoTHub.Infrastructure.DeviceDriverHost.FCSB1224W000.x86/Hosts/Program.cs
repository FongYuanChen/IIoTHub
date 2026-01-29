namespace IIoTHub.Infrastructure.DeviceDriverHost.FCSB1224W000.x86.Hosts
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "DeviceDriverHost.FCSB1224W000.x86";

            using var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;     // 不讓 process 直接被 kill
                cts.Cancel();
            };

            var server = new FCSB1224W000DriverPipeServer();

            Console.WriteLine("FCSB1224W000 Driver Host started.");
            await server.RunAsync(cts.Token);
        }
    }
}
