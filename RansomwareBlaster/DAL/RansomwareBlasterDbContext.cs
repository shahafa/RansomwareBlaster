using RansomwareBlaster.Models;

namespace RansomwareBlaster.DAL
{
    using System.Data.Entity;

    internal class RansomwareBlasterDbContext : DbContext
    {
        public RansomwareBlasterDbContext() : base("name=RansomwareBlasterDbContext")
        {
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<RansomwareBlasterDbContext>());
            //Database.SetInitializer<RansomwareBlasterDbContext>(new DropCreateDatabaseAlways<RansomwareBlasterDbContext>());
        }

        public virtual DbSet<Trap> Traps { get; set; }
    }
}