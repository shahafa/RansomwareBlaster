using System;
using System.IO;
using RansomwareBlaster.DAL;

namespace RansomwareBlaster.App
{
    internal class TrapsInitializer
    {
        public static void InitTraps(string directory = null)
        {
            // Clean DB
            using (var db = new RansomwareBlasterDbContext())
            {
                foreach (var trap in db.Traps)
                    db.Traps.Remove(trap);

                db.SaveChanges();
            }

            if (string.IsNullOrEmpty(directory))
            {
                foreach (var drive in Directory.GetLogicalDrives())
                {
                    try
                    {
                        SetTraps(drive);
                    }
                    catch (Exception)
                    {
                        // Todo: print log that adding trap to drive failed
                        // ignored
                    }
                }                
            }
            else
            {
                SetTraps(directory);
            }
        }


        private static void SetTraps(string directory)
        {
            var files = Directory.GetFiles(directory);

            foreach (var file in files)
            {
                // if file extension is not in ExtensionList we don't need to set trap for it
                var fileExtension = file.Substring(file.LastIndexOf(".", StringComparison.Ordinal) + 1);
                if (!Properties.Settings.Default.ExtensionsList.Contains(fileExtension)) continue;

                Traps.Create(directory);
            }

            var subDirectories = Directory.GetDirectories(directory);
            foreach (var subDirectory in subDirectories)
            {
                // if the directory is in the list of directories to ignore continue to the next directory
                if (Properties.Settings.Default.IgnoreDirectory.Contains(subDirectory)) continue;

                SetTraps(subDirectory);
            }
        }
    }
}
