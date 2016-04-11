using System;
using System.Diagnostics;
using System.Threading;

namespace AuditTester
{
    internal class Program
    {
        private static AutoResetEvent _signal;

        static void Main()
        {
            var eventLog = new EventLog("Security", ".", "Microsoft Windows security auditing.");
            eventLog.EntryWritten += MyOnEntryWritten;
            eventLog.EnableRaisingEvents = true;

            _signal = new AutoResetEvent(false);
            _signal.WaitOne();
        }

        public static void MyOnEntryWritten(object source, EntryWrittenEventArgs e)
        {

            if (!e.Entry.Source.Contains("Microsoft-Windows-Security-Auditing"))
                return;

            var processNameStartindex = e.Entry.Message.IndexOf("Process Name:", StringComparison.Ordinal);
            var processNameEndindex = e.Entry.Message.LastIndexOf("Access Request Information:", StringComparison.Ordinal);
            var processNameLineLength = processNameEndindex - processNameStartindex;
            var processNameLine = e.Entry.Message.Substring(processNameStartindex, processNameLineLength).Replace(" ", string.Empty);
            var processName = Convert.ToString(processNameLine.Split()[2]);

            // TODO: check if file is trap

            var pidStartindex = e.Entry.Message.IndexOf("Process ID:", StringComparison.Ordinal);
            var pidEndindex = e.Entry.Message.LastIndexOf("Process Name:", StringComparison.Ordinal);
            var pidLineLength = pidEndindex - pidStartindex;
            var pidLine = e.Entry.Message.Substring(pidStartindex, pidLineLength).Replace(" ", string.Empty);
            var pidString = Convert.ToString(pidLine.Split()[2].Remove(0, 2));
            var pid = Convert.ToInt32(pidString, 16);

            var malwareProcess = Process.GetProcessById(pid);
            malwareProcess.Kill();

            int i = 3;
        }
    }
}
