using System;
using Grapevine.CommandLine;
using Grpc.Core;
using Grpc.Net.Client;

const string address = "https://localhost:5001";

using var channel = GrpcChannel.ForAddress(address: address);
var trigonometryClient = new Trigonometry.TrigonometryClient(channel: channel);
var trigonometryRequest = new TrigonometryRequest { Start = 1, Count = 10000 };
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