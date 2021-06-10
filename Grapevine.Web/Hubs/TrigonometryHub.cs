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
        public async IAsyncEnumerable<int> Counter(
        int count,
        int delay,
        [EnumeratorCancellation]
        CancellationToken cancellationToken)
        {
            for (var i = 0; i < count; i++)
            {
                // Check the cancellation token regularly so that the server will stop
                // producing items if the client disconnects.
                cancellationToken.ThrowIfCancellationRequested();

                yield return i;

                // Use the cancellationToken in other APIs that accept cancellation
                // tokens so the cancellation can flow down to them.
                await Task.Delay(TimeSpan.FromMilliseconds(delay), cancellationToken);
            }
        }
    }
}