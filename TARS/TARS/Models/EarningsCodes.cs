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
    public class EarningsCodes
    {
        public int ID { get; set; } //DB iterator
        public string earningsCode { get; set; }     //3-Letter code
        public string description { get; set; }
    }

    public class EarningsCodesDBContext : DbContext
    {
        public DbSet<EarningsCodes> EarningsCodesList { get; set; }
        protected HistoryDBContext HistDB = new HistoryDBContext();

        public int SaveChanges()
        {
            //call our code here
            var hist = new History();
            ChangeTracker.DetectChanges();
            var holder = ChangeTracker.Entries<EarningsCodes>();
            foreach (System.Data.Entity.Infrastructure.DbEntityEntry<EarningsCodes> entry in holder)
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
                hist.dbtable = "EarningsCodes";
                hist.change = "EarningsCode: " + entry.Property(u => u.earningsCode).CurrentValue +
                                "Description: " + entry.Property(u => u.description).CurrentValue;
            }

            hist.username = System.Web.HttpContext.Current.User.Identity.Name;
            hist.timestamp = System.DateTime.Now;
            HistDB.HistoryList.Add(hist);
            HistDB.SaveChanges();

            return base.SaveChanges();
        }
    }
}