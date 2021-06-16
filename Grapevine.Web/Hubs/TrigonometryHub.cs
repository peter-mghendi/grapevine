using Grapevine.Core.Messages;
using Grapevine.Core.Services;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Grapevine.Web.Hubs
{
    public class TrigonometryHub : Hub
    {
        public async IAsyncEnumerable<TrigonometryReply> Trigonometries(
            TrigonometryRequest request,
            [EnumeratorCancellation] CancellationToken cancellationToken
        )
        {
            using var channel = GrpcChannel.ForAddress(address: "https://localhost:5001");
            var trigonometryClient = new Trigonometry.TrigonometryClient(channel: channel);

            // Use the cancellationToken in other APIs that accept cancellation
            // tokens so the cancellation can flow down to them.
            using var trigonometryCall = trigonometryClient.StreamTrigonometries(request: request, cancellationToken: cancellationToken);
            var replyStream = trigonometryCall.ResponseStream.ReadAllAsync(cancellationToken: cancellationToken);
            
            // This doesn't work
            // REF: https://stackoverflow.com/a/65921074
            // REF: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/async-streams#syntax-1
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