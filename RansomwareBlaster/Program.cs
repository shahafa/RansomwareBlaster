using System;
using System.IO;
using System.Threading;
using RansomwareBlaster.App;

namespace RansomwareBlaster
{
    internal class Program
    {
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        private static void Main()
        {
            InitializeDbDirectory();

            Console.CancelKeyPress += (sender, eArgs) => {
                QuitEvent.Set();
                eArgs.Cancel = true;
            };

            var trapsWatcher = new TrapsWatcher();
            trapsWatcher.Start();

            QuitEvent.WaitOne();
        }

        private static void InitializeDbDirectory()
        {
            var dbPath =
                $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\RansomwareBlaster\";

            if (!Directory.Exists(dbPath))
                Directory.CreateDirectory(dbPath);

            AppDomain.CurrentDomain.SetData("DataDirectory", dbPath);
        }
    }
}
