using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

using static Grapevine.Edge.FibonacciReply.Types;

namespace Grapevine.Edge
{
    public class FibonacciService : Fibonnaci.FibonnaciBase
    {
        private readonly ILogger<FibonacciService> _logger;
        public FibonacciService(ILogger<FibonacciService> logger) => _logger = logger;

        public override async Task StreamFibonacci(
            FibonacciRequest request,
            IServerStreamWriter<FibonacciReply> responseStream,
            ServerCallContext context
        )
        {
            for (int i = request.Start; i < request.Start + request.Count; i++)
            {
                var value = F(x: i);
                var reply = new FibonacciReply()
                {
                    Index = i,
                    Value = value,
                    Parity = value % 2 == 0 ? Parity.Even : Parity.Odd
                };

                _logger.LogInformation($"Computed value #${reply.Index}: ${value}");

                await responseStream.WriteAsync(reply);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private static int F(int x) => x < 2 ? x : F(x: x - 1) + F(x: x - 2);
    }
}
