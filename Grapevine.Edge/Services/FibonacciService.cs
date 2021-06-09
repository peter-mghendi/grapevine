using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grapevine.Edge.Services;
using Grpc.Core;
using Microsoft.Extensions.Logging;

using static Grapevine.Edge.FibonacciReply.Types;

namespace Grapevine.Edge
{
    public class FibonacciService : Fibonnaci.FibonnaciBase
    {
        private readonly ILogger<FibonacciService> _logger;

        private readonly IFibonacciCalculator _fibonacciCalculator;

        public FibonacciService(ILogger<FibonacciService> logger, IFibonacciCalculator fibonacciCalculator)
            => (_logger, _fibonacciCalculator) = (logger, fibonacciCalculator);

        public override async Task StreamFibonacci(
            FibonacciRequest request,
            IServerStreamWriter<FibonacciReply> responseStream,
            ServerCallContext context
        )
        {
            for (int i = request.Start; i < request.Start + request.Count; i++)
            {
                var value = _fibonacciCalculator.Calculate(x: i);
                var reply = new FibonacciReply()
                {
                    Index = i,
                    Value = value,
                    Parity = value % 2 == 0 ? Parity.Even : Parity.Odd
                };

                _logger.LogInformation($"Computed value #{reply.Index}: {value}");

                await responseStream.WriteAsync(reply);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
