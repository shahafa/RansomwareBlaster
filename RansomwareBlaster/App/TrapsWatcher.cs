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


        public TrapsWatcher()
        {
            _fileWatcher = new FileSystemWatcher
            {
                Path = @"C:\",
                Filter = "*.*",
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
            var createdFileExtension = (Path.GetExtension(fileSystemEventArgs.FullPath) ?? string.Empty).ToLower().Trim('.');

            // if file extension is not in _extensions list we don't need to monitor it
            if (!_extensions.Any(createdFileExtension.Equals)) return;

            var createdFileDirectory = fileSystemEventArgs.FullPath.Substring(0, fileSystemEventArgs.FullPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
            // TODO: According to traps algorithm decided if we need to create new trap or not
            if (!Traps.DirectoryContainsTrap(createdFileDirectory))
            {
                Traps.Create(createdFileDirectory);
            }
        }


        private void FileChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            // if file is not trap we don't need to monitor it
            // if (!TrapsOld.Instance.FileIsTrap(fileSystemEventArgs.FullPath)) return;

            // TODO: If trap file deleted update db and recreate new one

            // TODO: Kill own process
        }
    }
}
