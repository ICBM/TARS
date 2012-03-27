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
    public class TARSUser
    {
        public int ID { get; set; } //DB iterator.
        public string SID { get; set; }
        public string userName { get; set; }
        public string userID { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int permission { get; set; } //1 User   2 Manager   3 Admin 
        public string company { get; set; }
        public string department { get; set; }
        public string employeeOrContractor { get; set; }
        public char costAllocatedOrNot { get; set; }
    }

    public static class DateTimeExtensions
    {
        //method that returns the DateTime of the weekDay provided
        //example: DateTime.Now.StartOfWeek(DayOfWeek.Sunday) will return the DateTime of the Sunday of this week
        public static DateTime StartOfWeek(this DateTime referenceDate, DayOfWeek WeekDay)
        {
            int diff = referenceDate.DayOfWeek - WeekDay;
            if (diff < 0)
            {
                diff += 7;
            }
            return referenceDate.AddDays(-1 * diff).Date;
        }
    }

    public class TARSUserDBContext : DbContext
    {
        public DbSet<TARSUser> TARSUserList { get; set; }
        protected HistoryDBContext HistDB = new HistoryDBContext();

        public int SaveChanges()
        {
            //call our code here
            var hist = new History();
            ChangeTracker.DetectChanges();
            var holder = ChangeTracker.Entries<TARSUser>();
            foreach (System.Data.Entity.Infrastructure.DbEntityEntry<TARSUser> entry in holder)
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

                if (entry.Property(u => u.startDate).CurrentValue == null)
                {
                    entry.Property(u => u.startDate).CurrentValue = DateTime.MinValue;
                }
                if (entry.Property(u => u.endDate).CurrentValue == null)
                {
                    entry.Property(u => u.endDate).CurrentValue = DateTime.MaxValue;
                }

                hist.dbtable = "TARSUser";
                hist.change = "userName: " + entry.Property(u => u.userName).CurrentValue +
              "; permission: " + entry.Property(u => u.permission).CurrentValue;
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