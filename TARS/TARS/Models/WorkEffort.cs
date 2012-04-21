using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class WorkEffort
    {
        public int ID { get; set; } //DB iterator
        public string description { get; set; }
        public string comments { get; set; }
        public int pcaCode { get; set; }
        public bool hidden { get; set; }

        [DataType(DataType.Date)]
        public DateTime startDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime endDate { get; set; }
        public WorkEffort()
        {
            startDate = DateTime.Now;
            endDate = DateTime.Now;
        }
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
                hist.change = "description: " + entry.Property(u => u.description).CurrentValue +
                              "; comments: " + entry.Property(u => u.comments).CurrentValue +
                              "; pcaCode: " + entry.Property(u => u.pcaCode).CurrentValue +
                              "; startDate: " + entry.Property(u => u.startDate).CurrentValue.ToShortDateString() +
                              "; endDate: " + entry.Property(u => u.endDate).CurrentValue.ToShortDateString() +
                              "; hidden: " + entry.Property(u => u.hidden).CurrentValue;
            }

            hist.username = System.Web.HttpContext.Current.User.Identity.Name;
            hist.timestamp = System.DateTime.Now;
            HistDB.HistoryList.Add(hist);
            HistDB.SaveChanges();

            return base.SaveChanges();
        }
    }
}