using System;
using System.Linq;
using RansomwareBlaster.Models;

namespace RansomwareBlaster.DAL
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

        public static void Create(string directory)
        {
            using (var db = new RansomwareBlasterDbContext())
            {
                db.Traps.Add(new Trap()
                {
                    ID = Guid.NewGuid().ToString(),
                    Directory = directory,
                    FileName = "$$.doc"
                });

                db.SaveChanges();
            }
        }
    }
}