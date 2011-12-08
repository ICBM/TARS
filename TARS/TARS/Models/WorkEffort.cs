using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class WorkEffort
    {
        public int ID { get; set; } //DB iterator?
        public int code { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string creator { get; set; }
        public string description { get; set; }
        public string files { get; set; }
        public bool active { get; set; }
    }

    public class WorkEffortDBContext : DbContext
    {
        public DbSet<WorkEffort> WorkEffortList { get; set; }
        protected HistoryDBContext HistDB = new HistoryDBContext();

        public int SaveChanges()
        {
            //call our code here
            var hist = new History();
            ChangeTracker.DetectChanges();
            var holder = ChangeTracker.Entries<WorkEffort>();
            foreach (System.Data.Entity.Infrastructure.DbEntityEntry<WorkEffort> entry in holder)
            {
                switch (entry.State)
                {
                    case System.Data.EntityState.Added:
                        hist.type = "added";
                        break;
                    case System.Data.EntityState.Deleted:
                        hist.type = "deleted";
                        break;
                    case System.Data.EntityState.Modified:
                        hist.type = "modified";
                        break;
                }
                hist.dbtable = "WorkEfforts";
                hist.change = "code: " + entry.Property(u => u.code).CurrentValue +
              "; startDate: " + entry.Property(u => u.startDate).CurrentValue +
              "; endDate: " + entry.Property(u => u.endDate).CurrentValue +
              "; creator: " + entry.Property(u => u.creator).CurrentValue +
              "; description: " + entry.Property(u => u.description).CurrentValue +
              "; files: " + entry.Property(u => u.files).CurrentValue +
              "; active: " + entry.Property(u => u.active).CurrentValue;
            }

            //Doesn't actually get the current user's name.  User.Identity.Name doesn't work here
            hist.username = "placeholder";

            hist.timestamp = System.DateTime.Now;
            HistDB.HistoryList.Add(hist);
            HistDB.SaveChanges();

            return base.SaveChanges();
        }
    }
}