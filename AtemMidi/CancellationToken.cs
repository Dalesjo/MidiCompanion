using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiCompanion;
public static class CancellationTokenExtensions
{
    public static Task WaitForCancellationAsync(this CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<object?>();

        cancellationToken.Register(() => tcs.SetResult(null));

        return tcs.Task;
    }
}