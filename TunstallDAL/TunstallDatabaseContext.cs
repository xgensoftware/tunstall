using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using TunstallDAL.Entities;
namespace TunstallDAL
{
    public class TunstallDatabaseContext : DbContext
    {
        public TunstallDatabaseContext():base("name=TunstallDB")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventCodeMapping> EventCodeMappings { get; set; }
    }
}
