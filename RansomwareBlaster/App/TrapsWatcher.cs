using System;
using System.IO;
using System.Linq;
using RansomwareBlaster.DAL;

namespace RansomwareBlaster.App
{
    internal class TrapsWatcher
    {
        private readonly FileSystemWatcher _fileWatcher;
        private readonly string[] _extensions = { "txt", "doc", "docx", "xls", "xlsx", "pdf", "png", "jpg" };
        private readonly string _dbFile;
        private DateTime _fileChangedLastReadTime = DateTime.MinValue;
        private DateTime _fileCreatedLastReadTime = DateTime.MinValue;
        private DateTime _fileRenamedLastReadTime = DateTime.MinValue;
        private const string SystemVolumeInformationDirectory = @"C:\System Volume Information\";

        public TrapsWatcher()
        {
            _dbFile = $"{AppDomain.CurrentDomain.GetData("DataDirectory")}db.sdf";

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


        private void FileCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            // To prevent multiple event calls we compare last write time with last read time
            var lastWriteTime = File.GetLastWriteTime(fileSystemEventArgs.FullPath);
            if (lastWriteTime == _fileCreatedLastReadTime) return;
            _fileCreatedLastReadTime = lastWriteTime;

            // if file extension is not in _extensions list we don't need to monitor it
            var createdFileExtension = (Path.GetExtension(fileSystemEventArgs.FullPath) ?? string.Empty).ToLower().Trim('.');
            if (!_extensions.Any(createdFileExtension.Equals)) return;

            // create new trap
            var createdFileDirectory = fileSystemEventArgs.FullPath.Substring(0, fileSystemEventArgs.FullPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
            Traps.Create(createdFileDirectory);
        }


        private void FileRenamed(object sender, RenamedEventArgs fileSystemEventArgs)
        {
            // To prevent multiple event calls we compare last write time with last read time
            var lastWriteTime = File.GetLastWriteTime(fileSystemEventArgs.FullPath);
            if (lastWriteTime == _fileRenamedLastReadTime) return;
            _fileRenamedLastReadTime = lastWriteTime;

            // if file extension is not in _extensions list we don't need to monitor it
            var renamedFileExtension = (Path.GetExtension(fileSystemEventArgs.OldFullPath) ?? string.Empty).ToLower().Trim('.');
            if (!_extensions.Any(renamedFileExtension.Equals)) return;

            // If renamed file is trap than malware detected
            if (Traps.IsTrap(fileSystemEventArgs.OldFullPath))
            {
                MalwareDetected();
            }
        }


        private void FileChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            // Ignore changes in db file to improve performance and system volume information dir
            if (fileSystemEventArgs.FullPath.Equals(_dbFile) ||
                fileSystemEventArgs.FullPath.StartsWith(SystemVolumeInformationDirectory))
                return;

            // To prevent multiple event calls we compare last write time with last read time
            var lastWriteTime = File.GetLastWriteTime(fileSystemEventArgs.FullPath);
            if (lastWriteTime == _fileChangedLastReadTime) return;
            _fileChangedLastReadTime = lastWriteTime;

            // if file extension is not in _extensions list we don't need to monitor it
            var changedFileExtension = Path.GetExtension(fileSystemEventArgs.FullPath).ToLower().Trim('.');
            if (!_extensions.Any(changedFileExtension.Equals)) return;

            // if trap changed than malware detected
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
