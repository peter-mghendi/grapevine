using System;
using System.Threading;
using System.Threading.Tasks;
using Grapevine.CommandLine;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.SignalR.Client;

Request request = new(Start: 1, Count: 100, Delay: 500);
CancellationTokenSource cts = new(); // TODO 

await StreamFromGrpcServerAsync(request: request, cancellationToken: cts.Token);
await StreamFromSignalRServerAsync(request: request, cancellationToken: cts.Token);

static async Task StreamFromGrpcServerAsync(Request request, CancellationToken cancellationToken)
{
    using var channel = GrpcChannel.ForAddress(address: "https://localhost:5001");
    var trigonometryClient = new Trigonometry.TrigonometryClient(channel: channel);
    var trigonometryRequest = new TrigonometryRequest
    {
        Start = request.Start,
        Count = request.Count,
        Delay = request.Delay
    };

    using var trigonometryCall = trigonometryClient.StreamTrigonometries(request: trigonometryRequest);
    var stream = trigonometryCall.ResponseStream.ReadAllAsync(cancellationToken: cancellationToken);

    await foreach (var reply in stream) DisplayTrigonometries(reply: reply);
}

static async Task StreamFromSignalRServerAsync(Request request, CancellationToken cancellationToken)
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
        arg1: request.Start,
        arg2: request.Count,
        arg3: request.Delay,
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

// TODO: Just use TriginometryRequest everywhere
record Request(int Start, int Count, int Delay);