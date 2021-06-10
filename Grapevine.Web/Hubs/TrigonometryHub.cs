using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Grapevine.Web.Hubs
{
    public class TrigonometryHub : Hub
    {
        public async IAsyncEnumerable<TrigonometryReply> Trigonometries(
            int start,
            int count,
            int delay,
            [EnumeratorCancellation] CancellationToken cancellationToken
        )
        {
            using var channel = GrpcChannel.ForAddress(address: "https://localhost:5001");
            var trigonometryClient = new Trigonometry.TrigonometryClient(channel: channel);
            var trigonometryRequest = new TrigonometryRequest { Start = start, Count = count, Delay = delay };

            // Use the cancellationToken in other APIs that accept cancellation
            // tokens so the cancellation can flow down to them.
            using var trigonometryCall = trigonometryClient.StreamTrigonometries(request: trigonometryRequest, cancellationToken: cancellationToken);
            var replyStream = trigonometryCall.ResponseStream.ReadAllAsync(cancellationToken: cancellationToken);
            
            // return replyStream;

            await foreach (var trigonometryReply in replyStream)
            {
                // Check the cancellation token regularly so that the server will stop
                // producing items if the client disconnects.
                cancellationToken.ThrowIfCancellationRequested();
                yield return trigonometryReply;
            }
        }
    }
}