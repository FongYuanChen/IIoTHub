namespace IIoTHub.Infrastructure.DeviceDriverHost.Focas.x86.Hosts
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "DeviceDriverHost.Focas.x86";

            using var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;     // 不讓 process 直接被 kill
                cts.Cancel();
            };

            var server = new FocasDriverPipeServer();

            Console.WriteLine("FANUC Driver Host started.");
            await server.RunAsync(cts.Token);
        }
    }
}
