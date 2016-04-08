using System;
using System.IO;
using RansomwareBlaster.Models;

namespace RansomwareBlaster.DAL
{
    using System.Data.Entity;

    internal class RansomwareBlasterDbContext : DbContext
    {
        public RansomwareBlasterDbContext() : base("name=RansomwareBlasterDbContext")
        {
            var dbPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\RansomwareBlaster\";

            if (!Directory.Exists(dbPath))
                Directory.CreateDirectory(dbPath);

            AppDomain.CurrentDomain.SetData("DataDirectory", dbPath);

            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<RansomwareBlasterDbContext>());
            //Database.SetInitializer(new DropCreateDatabaseAlways<RansomwareBlasterDbContext>());
        }

        public virtual DbSet<Trap> Traps { get; set; }
    }
}