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
        private DateTime _fileCreatedOrRenamedLastReadTime = DateTime.MinValue;

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
            _fileWatcher.Created += FileCreatedOrRenamed;
            _fileWatcher.Renamed += FileCreatedOrRenamed;
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


        private void FileCreatedOrRenamed(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            var lastWriteTime = File.GetLastWriteTime(fileSystemEventArgs.FullPath);
            if (lastWriteTime == _fileCreatedOrRenamedLastReadTime)
                return;

            _fileCreatedOrRenamedLastReadTime = lastWriteTime;

            var createdFileExtension = (Path.GetExtension(fileSystemEventArgs.FullPath) ?? string.Empty).ToLower().Trim('.');

            // if file extension is not in _extensions list we don't need to monitor it
            if (!_extensions.Any(createdFileExtension.Equals)) return;

            // TODO: According to traps algorithm decided if we need to create new trap or not

            var createdFileDirectory = fileSystemEventArgs.FullPath.Substring(0, fileSystemEventArgs.FullPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
            if (!Traps.DirectoryContainsTrap(createdFileDirectory))
            {
                Traps.Create(createdFileDirectory);
            }
        }

        private void FileChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            // To prevent multiple event calls we compare last write time with last read time
            var lastWriteTime = File.GetLastWriteTime(fileSystemEventArgs.FullPath);
            if (lastWriteTime == _fileChangedLastReadTime)
                return;

            _fileChangedLastReadTime = lastWriteTime;

            // Ignore changes in db file to improve performance
            if (fileSystemEventArgs.FullPath.Equals(_dbFile)) return;

            if (!Traps.IsTrap(fileSystemEventArgs.FullPath)) return;

            Console.Out.WriteLine("Ransomware Detected!!!");
            // TODO: If trap file deleted update db and recreate new one

            // TODO: Kill own process
        }
    }
}
