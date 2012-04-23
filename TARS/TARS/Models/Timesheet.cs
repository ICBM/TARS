using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class Timesheet
    {
        public int ID { get; set; } //DB iterator
        public string worker { get; set; }
        public bool submitted { get; set; }
        public bool approved { get; set; }
        public bool locked { get; set; }   //can be locked with or without being approved

        [DataType(DataType.Date)]
        public DateTime periodStart { get; set; }
        [DataType(DataType.Date)]
        public DateTime periodEnd { get; set; }
    }

    public class TimesheetDBContext : DbContext
    {
        public DbSet<Timesheet> TimesheetList { get; set; }
        protected HistoryDBContext HistDB = new HistoryDBContext();

        public int SaveChanges()
        {
            //call our code here
            var hist = new History();
            ChangeTracker.DetectChanges();
            var holder = ChangeTracker.Entries<Timesheet>();
            foreach (System.Data.Entity.Infrastructure.DbEntityEntry<Timesheet> entry in holder)
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
                hist.dbtable = "Timesheets";
                hist.change = "worker: " + entry.Property(u => u.worker).CurrentValue +
                              "; periodStart: " + entry.Property(u => u.periodStart).CurrentValue.ToShortDateString() +
                              "; periodEnd: " + entry.Property(u => u.periodEnd).CurrentValue.ToShortDateString() +
                              "; submitted: " + entry.Property(u => u.submitted).CurrentValue +
                              "; approved: " + entry.Property(u => u.approved).CurrentValue +
                              "; locked: " + entry.Property(u => u.locked).CurrentValue;
            }

            hist.username = System.Web.HttpContext.Current.User.Identity.Name;
            hist.timestamp = System.DateTime.Now;
            HistDB.HistoryList.Add(hist);
            HistDB.SaveChanges();

            return base.SaveChanges();
        }
    }
}