using System.Threading;

namespace MvvmCross.Plugins.BLE.Extensions
{
    public static class CancellationTokenSourceExtensions
    {
        public static void CancelIfNotAlreadyRequested(this CancellationTokenSource cts)
        {
            if (!cts.IsCancellationRequested)
            {
                cts.Cancel();
            }
        }
    }
}