using System;
using System.Threading;
using System.Threading.Tasks;
using Grapevine.Core.Messages;
using Grapevine.Core.Services;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.SignalR.Client;

TrigonometryRequest request = new() { Start = 1, Count = 100, Delay = 500 };
CancellationTokenSource cts = new(); // TODO 

await StreamFromGrpcServerAsync(request: request, cancellationToken: cts.Token);
await StreamFromSignalRServerAsync(request: request, cancellationToken: cts.Token);

static async Task StreamFromGrpcServerAsync(TrigonometryRequest request, CancellationToken cancellationToken)
{
    using var channel = GrpcChannel.ForAddress(address: "https://localhost:5001");
    var trigonometryClient = new Trigonometry.TrigonometryClient(channel: channel);

    using var trigonometryCall = trigonometryClient.StreamTrigonometries(request: request);
    var stream = trigonometryCall.ResponseStream.ReadAllAsync(cancellationToken: cancellationToken);

    await foreach (var reply in stream) DisplayTrigonometries(reply: reply);
}

static async Task StreamFromSignalRServerAsync(TrigonometryRequest request, CancellationToken cancellationToken)
{
    await using var connection = new HubConnectionBuilder()
        .WithUrl(url: "https://localhost:5003/trigonometry")
        .Build();

    connection.Closed += async _ =>
    {
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken: cancellationToken);
        await connection.StartAsync(cancellationToken: cancellationToken);
    };

    await connection.StartAsync(cancellationToken: cancellationToken);
    var stream = connection.StreamAsync<TrigonometryReply>(
        methodName: "Trigonometries",
        arg1: request,
        cancellationToken: cancellationToken
    );

    await foreach (var reply in stream) DisplayTrigonometries(reply: reply);
}

static void DisplayTrigonometries(TrigonometryReply reply)
    => Console.WriteLine($"Time\t: {DateTime.Now} \n"
        + $"Num\t: {reply.Num} \n"
        + $"Sin\t: {reply.Sin} \n"
        + $"Cos\t: {reply.Cos} \n"
        + $"Tan\t: {reply.Tan} \n");