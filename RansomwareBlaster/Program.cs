using System;
using System.Threading;
using RansomwareBlaster.App;

namespace RansomwareBlaster
{
    internal class Program
    {
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        private static void Main()
        {
            Console.CancelKeyPress += (sender, eArgs) => {
                QuitEvent.Set();
                eArgs.Cancel = true;
            };

            var trapsWatcher = new TrapsWatcher();
            trapsWatcher.Start();

            QuitEvent.WaitOne();
        }
    }
}
