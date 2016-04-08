using System;
using System.IO;
using System.Linq;
using RansomwareBlaster.DAL;

namespace RansomwareBlaster.App
{
    internal class TrapsInitializer
    {
        private const string RootDirectory = @"C:\RansomwarePlayground\";
        private static readonly string[] Extensions = { "txt", "doc", "docx", "xls", "xlsx", "pdf", "png", "jpg" };
        private static readonly string[] IgnoreDirectory = { @"C:\Program Files", @"C:\Program Files (x86)", @"C:\Windows", @"C:\$Recycle.Bin" };

        public static void InitTraps()
        {
            using (var db = new RansomwareBlasterDbContext())
            {
                foreach (var trap in db.Traps)
                    db.Traps.Remove(trap);
                db.SaveChanges();
            }

            // Todo: Change to run on each drive of the system
            SetTraps(RootDirectory);
        }

        private static void SetTraps(string directory)
        {
            var files = Directory.GetFiles(directory);

            foreach (var file in files)
            {
                var fileExtension = file.Substring(file.LastIndexOf(".", StringComparison.Ordinal) + 1);
                if (!Extensions.Any(fileExtension.Equals)) continue;

                Traps.Create(directory);
            }

            var subDirectories = Directory.GetDirectories(directory);
            foreach (var subDirectory in subDirectories)
            {
                if (IgnoreDirectory.Any(subDirectory.Equals))
                    continue;

                SetTraps(subDirectory);
            }
        }
    }
}
