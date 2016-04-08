using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using RansomwareBlaster.App;

namespace RansomwareBlaster
{
    internal class Program
    {
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        private static void Main(string[] args)
        {
            if (args.Any(arg => arg.Equals("init")))
            {
                TrapsInitializer.InitTraps();
            }
            else
            {
                Run();
            }
        }

        private static void Run()
        {
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                QuitEvent.Set();
                eArgs.Cancel = true;
            };

            var trapsWatcher = new TrapsWatcher();
            trapsWatcher.Start();

            QuitEvent.WaitOne();
        }
    }
}