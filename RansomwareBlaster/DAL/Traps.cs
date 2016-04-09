using System;
using System.Linq;
using RansomwareBlaster.Models;

namespace RansomwareBlaster.Dal
{
    public class Traps
    {
        public static bool DirectoryContainsTrap(string directory)
        {
            using (var db = new RansomwareBlasterDbContext())
            {
                return db.Traps.Any(trap => trap.Directory.Equals(directory));
            }
        }

        public static bool IsTrap(string file)
        {
            var directory = file.Substring(0, file.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
            var fileName = file.Substring(file.LastIndexOf(@"\", StringComparison.Ordinal) + 1);

            using (var db = new RansomwareBlasterDbContext())
            {
                var trapFiles = db.Traps.Where(trap => trap.Directory.Equals(directory));

                return trapFiles.Any(trap => trap.FileName.Equals(fileName));
            }
        }

        public static void Create(string directory)
        {
            // TODO: According to traps algorithm decided if we need to create new trap or not
            //       a. Create trap according to dictionary before and after every new file
            //       b. Create 2 trap files according to dictionary first and last
            //       c. Create few traps only in specific places

            if (DirectoryContainsTrap(directory)) return;

            using (var db = new RansomwareBlasterDbContext())
            {
                System.IO.File.WriteAllText($@"{directory}\$$.txt", "This is a trap file!!!");

                db.Traps.Add(new Trap()
                {
                    ID = Guid.NewGuid().ToString(),
                    Directory = directory,
                    FileName = "$$.txt"
                });

                db.SaveChanges();
            }
        }
    }
}