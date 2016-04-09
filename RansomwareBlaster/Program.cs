using System;
using System.Threading;
using RansomwareBlaster.App;
using RansomwareBlaster.DAL;

namespace RansomwareBlaster
{
    internal class Program
    {
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        private static void Main(string[] args)
        {
            RansomwareBlasterDbContext.InitializeDb();

            if (args.Length == 0)
            {
                Console.WriteLine(@"Usage: blaster [OPTIONS] COMMAND [args...]");
                Console.WriteLine(@"");
                Console.WriteLine(@"Options:");
                Console.WriteLine(@"  -s                Silent mode");
                Console.WriteLine(@"  -v, --version     Print version information and quit");
                Console.WriteLine(@"");
                Console.WriteLine(@"Commmands:");
                Console.WriteLine(@"  init              Init trap files");
                Console.WriteLine(@"  monitor           Monitor trap files");
                Console.WriteLine(@"");
                Console.WriteLine(@"Run 'blaster COMMAND --help' for more information on a command");
            }
            else if (args[0].Equals("init"))
            {
                if (args.Length == 1)
                {
                    TrapsInitializer.InitTraps();
                }
                else
                {
                    TrapsInitializer.InitTraps(args[1]);
                }
            }
            else if (args[0].Equals("monitor"))
            {
                Monitor();
            }
        }

        private static void Monitor()
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