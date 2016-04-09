using System;
using System.IO;
using RansomwareBlaster.Models;

namespace RansomwareBlaster.Dal
{
    using System.Data.Entity;

    internal class RansomwareBlasterDbContext : DbContext
    {
        public RansomwareBlasterDbContext() : base("name=RansomwareBlasterDbContext")
        {
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<RansomwareBlasterDbContext>());
            //Database.SetInitializer(new DropCreateDatabaseAlways<RansomwareBlasterDbContext>());
        }

        public static void InitializeDb()
        {
            var dbPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\RansomwareBlaster\";

            if (!Directory.Exists(dbPath))
                Directory.CreateDirectory(dbPath);

            AppDomain.CurrentDomain.SetData("DataDirectory", dbPath);
        }

        public virtual DbSet<Trap> Traps { get; set; }
    }
}