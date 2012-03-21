using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Text;
using System.Collections;
using System.DirectoryServices;

using TARS.Helpers;
using TARS.Models;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class Timesheet
    {
        public int ID { get; set; } //DB iterator
        public string worker { get; set; }
        public DateTime periodStart { get; set; }
        public DateTime periodEnd { get; set; }
        public bool approved { get; set; }
        public bool locked { get; set; }   //can be locked with or without being approved
    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }
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
                hist.dbtable = "Timesheet";
                hist.change = "worker: " + entry.Property(u => u.worker).CurrentValue +
              "; periodStart: " + entry.Property(u => u.periodStart).CurrentValue +
              "; periodEnd: " + entry.Property(u => u.periodEnd).CurrentValue +
              "; approved: " + entry.Property(u => u.approved).CurrentValue +
              "; locked: " + entry.Property(u => u.locked).CurrentValue;
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