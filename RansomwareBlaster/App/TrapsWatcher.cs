using System;
using System.IO;
using System.Linq;
using RansomwareBlaster.DAL;

namespace RansomwareBlaster.App
{
    internal class TrapsWatcher
    {
        private readonly FileSystemWatcher _fileWatcher;
        private readonly string _dbFile;
        private DateTime _fileLastReadTime;
        
        public TrapsWatcher()
        {
            _dbFile = $"{AppDomain.CurrentDomain.GetData("DataDirectory")}db.sdf";
            _fileLastReadTime = DateTime.MinValue;

            _fileWatcher = new FileSystemWatcher
            {
                Path = @"C:\",
                Filter = "*.*",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                IncludeSubdirectories = true
            };
            _fileWatcher.Created += FileCreated;
            _fileWatcher.Renamed += FileRenamed;
            _fileWatcher.Changed += FileChanged;
            _fileWatcher.Deleted += FileChanged;
        }


        public void Start()
        {
            _fileWatcher.EnableRaisingEvents = true;
        }


        public void Stop()
        {
            _fileWatcher.EnableRaisingEvents = false;
        }

        private bool MonitoredFile(string fileFullPath)
        {
            // To prevent multiple event calls we compare last write time with last read time
            var lastWriteTime = File.GetLastWriteTime(fileFullPath);
            if (lastWriteTime == _fileLastReadTime) return false;
            _fileLastReadTime = lastWriteTime;

            // Ignore changes in db file to improve performance and system volume information dir
            if (fileFullPath.Equals(_dbFile))
                return false;

            // if file extension is not in ExtensionsList we don't need to monitor it
            var fileExtension = Path.GetExtension(fileFullPath).ToLower().Trim('.');
            if (!Properties.Settings.Default.ExtensionsList.Contains(fileExtension))
                return false;

            // If directory is in IgnoreDirectory list ignore the change
            var createdFileDirectory = fileFullPath.Substring(0, fileFullPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
            if (Properties.Settings.Default.IgnoreDirectory.Cast<string>().Any(ignoreDirectory => createdFileDirectory.StartsWith(ignoreDirectory)))
                return false;

            return true;
        }


        private void FileCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            if (!MonitoredFile(fileSystemEventArgs.FullPath)) return;

            // create new trap
            var createdFileDirectory = fileSystemEventArgs.FullPath.Substring(0, fileSystemEventArgs.FullPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
            Traps.Create(createdFileDirectory);
        }


        private void FileRenamed(object sender, RenamedEventArgs fileSystemEventArgs)
        {
            if (!MonitoredFile(fileSystemEventArgs.OldFullPath)) return;

            // If trap file renamed than malware detected
            if (Traps.IsTrap(fileSystemEventArgs.OldFullPath))
            {
                MalwareDetected();
            }
        }


        private void FileChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            if (!MonitoredFile(fileSystemEventArgs.FullPath)) return;

            // if trap file changed than malware detected
            if (Traps.IsTrap(fileSystemEventArgs.FullPath))
            {
                MalwareDetected();
            }
        }


        private void MalwareDetected()
        {
            Console.Out.WriteLine("Ransomware Detected!!!");

            // TODO: If trap file deleted update db and recreate new one

            // TODO: Kill own process
        }
    }
}
