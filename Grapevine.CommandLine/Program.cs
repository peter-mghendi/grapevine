using System;
using System.Threading;
using System.Threading.Tasks;
using Grapevine.CommandLine;
using Grpc.Core;
using Grpc.Net.Client;

// TODO CancellationToken support

await StreamFromGrpcServerAsync();
// await StreamFromSignalRServerAsync();

static async Task StreamFromGrpcServerAsync(CancellationToken cancellationToken = default)
{
    using var channel = GrpcChannel.ForAddress(address: "https://localhost:5001");
    var trigonometryClient = new Trigonometry.TrigonometryClient(channel: channel);
    var trigonometryRequest = new TrigonometryRequest { Start = 1, Count = 10000, Delay = 500 };

    using var trigonometryCall = trigonometryClient.StreamTrigonometries(request: trigonometryRequest);
    await foreach (var trigonometryReply in trigonometryCall.ResponseStream.ReadAllAsync())
    {
        var message = $"Time\t: {DateTime.Now} \n"
            + $"Num\t: {trigonometryReply.Num} \n"
            + $"Sin\t: {trigonometryReply.Sin} \n"
            + $"Cos\t: {trigonometryReply.Cos} \n"
            + $"Tan\t: {trigonometryReply.Tan} \n";

        Console.WriteLine(message);
    }
}

// static async Task StreamFromSignalRServerAsync(CancellationToken cancellationToken = default)
// {
//     throw new NotImplementedException();
// }