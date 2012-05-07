using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using TARS.Helpers;
using TARS.Models;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class Hours
    {
        public int ID { get; set; } //DB iterator
        public int workEffortID { get; set; }
        public string description { get; set; } 
        public string creator { get; set; }
        public double hours { get; set; }

        [DataType(DataType.Date)]
        public DateTime timestamp { get; set; }
        public Hours()
        {
            timestamp = DateTime.Now;
        }
    }

    public class HoursDBContext : DbContext
    {
        public DbSet<Hours> HoursList { get; set; }
        protected HistoryDBContext HistDB = new HistoryDBContext();

        public int SaveChanges()
        {
            //call our code here
            var hist = new History();
            ChangeTracker.DetectChanges();
            var holder = ChangeTracker.Entries<Hours>();
            foreach (System.Data.Entity.Infrastructure.DbEntityEntry<Hours> entry in holder)
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
                hist.dbtable = "Hours";
                hist.change = "workEffortID: " + entry.Property(u => u.workEffortID).CurrentValue +
                              "; timeCode: " + entry.Property(u => u.description).CurrentValue +
                              "; hours: " + entry.Property(u => u.hours).CurrentValue +
                              "; date: " + entry.Property(u => u.timestamp).CurrentValue.ToShortDateString() +
                              "; creator: " + entry.Property(u => u.creator).CurrentValue;
            }

            hist.username = System.Web.HttpContext.Current.User.Identity.Name;
            hist.timestamp = System.DateTime.Now;
            HistDB.HistoryList.Add(hist);
            HistDB.SaveChanges();

            return base.SaveChanges();
        }
    }
}