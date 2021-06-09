using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grapevine.CommandLine;
using Grpc.Core;
using Grpc.Net.Client;

const string address = "https://localhost:5001";

using var channel = GrpcChannel.ForAddress(address: address);

var greeterClient = new Greeter.GreeterClient(channel: channel);
var helloRequest = new HelloRequest { Name = "CommandLine" };
var helloReply = await greeterClient.SayHelloAsync(request: helloRequest);

Console.WriteLine("Greeting: " + helloReply.Message);
Console.WriteLine("Press any key to continue...");
Console.ReadKey();

var fibonacciClient = new Fibonnaci.FibonnaciClient(channel: channel);
var fibonacciRequest = new FibonacciRequest { Start = 1, Count = 100 };
using var fibonacciCall = fibonacciClient.StreamFibonacci(request: fibonacciRequest);

await foreach (var fibonacciReply in fibonacciCall.ResponseStream.ReadAllAsync())
{
    var message = $"Time\t: {DateTime.Now} \n"
        + $"Index\t: {fibonacciReply.Index} \n"
        + $"Value\t: {fibonacciReply.Value} \n" 
        + $"Parity\t: {fibonacciReply.Parity}\n";
    Console.WriteLine(message);
}