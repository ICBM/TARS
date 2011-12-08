using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using System.Data.Entity;
using System.Web.Mvc;

namespace TARS.Models
{
    public class Task
    {    
        public int ID { get; set; } //DB iterator
        public int WE { get; set; }
        public string title { get; set; }
    }

    public class TaskDBContext : DbContext
    {
        public DbSet<Task> TaskList { get; set; }
        protected HistoryDBContext HistDB = new HistoryDBContext();

        public int SaveChanges()
        {
            //call our code here
            var hist = new History();
            ChangeTracker.DetectChanges();
            var holder = ChangeTracker.Entries<Task>();
            foreach (System.Data.Entity.Infrastructure.DbEntityEntry<Task> entry in holder)
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
                hist.dbtable = "Tasks";
                hist.change = "WE: " + entry.Property(u => u.WE).CurrentValue +
              "; title: " + entry.Property(u => u.title).CurrentValue;
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
