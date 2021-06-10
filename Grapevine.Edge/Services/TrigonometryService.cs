using System;
using System.Threading.Tasks;
using Grapevine.Edge;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Grapevine.Edge.Services
{
    public class TrigonometryService : Trigonometry.TrigonometryBase
    {
        private readonly ILogger<TrigonometryService> _logger;

        public TrigonometryService(ILogger<TrigonometryService> logger) => _logger = logger;

        public override async Task StreamTrigonometries(
            TrigonometryRequest request,
            IServerStreamWriter<TrigonometryReply> responseStream,
            ServerCallContext context
        )
        {
            for (int num = request.Start; num < request.Start + request.Count; num++)
            {
                // Check the cancellation token regularly so that the server will stop
                // producing items if the client disconnects.
                context.CancellationToken.ThrowIfCancellationRequested();

                await responseStream.WriteAsync(message: new()
                {
                    Num = num,
                    Sin = Math.Sin(num),
                    Cos = Math.Cos(num),
                    Tan = Math.Sin(num),
                });

                // Use the cancellationToken in other APIs that accept cancellation
                // tokens so the cancellation can flow down to them.
                await Task.Delay(TimeSpan.FromMilliseconds(request.Delay), context.CancellationToken);
            }
        }
    }
}
