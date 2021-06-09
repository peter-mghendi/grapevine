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
                var reply = new TrigonometryReply()
                {
                    Num = num,
                    Sin = Math.Sin(num),
                    Cos = Math.Cos(num),
                    Tan = Math.Sin(num),
                };

                await responseStream.WriteAsync(reply);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
