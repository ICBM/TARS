using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using TARS.Helpers;
using TARS.Models;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class Holidays
    {
        public int ID { get; set; } //DB iterator
        [Required]
        [StringLength(50, ErrorMessage = "Must be 50 characters or less")]
        public string holidayName { get; set; }
        [DataType (DataType.Date)]
        public DateTime? date { get; set; }

        public Holidays()
        {
            date = DateTime.MinValue;
        }
    }

    public class HolidaysDBContext : DbContext
    {
        public DbSet<Holidays> HolidaysList { get; set; }
        protected HistoryDBContext HistDB = new HistoryDBContext();

        public int SaveChanges()
        {
            //call our code here
            var hist = new History();
            ChangeTracker.DetectChanges();
            var holder = ChangeTracker.Entries<Holidays>();
            foreach (System.Data.Entity.Infrastructure.DbEntityEntry<Holidays> entry in holder)
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
                hist.dbtable = "Holidays";
                hist.change = "Holiday: " + entry.Property(u => u.holidayName).CurrentValue +
                              "; Date: " + entry.Property(u => u.date).CurrentValue;
            }

            hist.username = System.Web.HttpContext.Current.User.Identity.Name;
            hist.timestamp = System.DateTime.Now;
            HistDB.HistoryList.Add(hist);
            HistDB.SaveChanges();

            return base.SaveChanges();
        }
    }
}