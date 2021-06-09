using System;
using Grapevine.CommandLine;
using Grpc.Core;
using Grpc.Net.Client;

const string address = "https://localhost:5001";

using var channel = GrpcChannel.ForAddress(address: address);

// var greeterClient = new Greeter.GreeterClient(channel: channel);
// var helloRequest = new HelloRequest { Name = "CommandLine" };
// var helloReply = await greeterClient.SayHelloAsync(request: helloRequest);

// Console.WriteLine("Greeting: " + helloReply.Message);
// Console.WriteLine("Press any key to continue...");
// Console.ReadKey();

// var fibonacciClient = new Fibonnaci.FibonnaciClient(channel: channel);
// var fibonacciRequest = new FibonacciRequest { Start = 1, Count = 40 };
// using var fibonacciCall = fibonacciClient.StreamFibonacci(request: fibonacciRequest);

// await foreach (var fibonacciReply in fibonacciCall.ResponseStream.ReadAllAsync())
// {
//     var message = $"Time\t: {DateTime.Now} \n"
//         + $"Index\t: {fibonacciReply.Index} \n"
//         + $"Value\t: {fibonacciReply.Value} \n" 
//         + $"Parity\t: {fibonacciReply.Parity}\n";
//     Console.WriteLine(message);
// }

var trigonometryClient = new Trigonometry.TrigonometryClient(channel: channel);
var trigonometryRequest = new TrigonometryRequest { Start = 1, Count = 40 };
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