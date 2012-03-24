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
    public class PcaCode
    {    
        public int ID { get; set; } //DB iterator
        public int code { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string description { get; set; }
        public string division { get; set; }
        public bool active { get; set; }
    }

    public class PcaCodeDBContext : DbContext
    {
        public DbSet<PcaCode> PcaCodeList { get; set; }
        protected HistoryDBContext HistDB = new HistoryDBContext();

        public int SaveChanges()
        {
            //call our code here
            var hist = new History();
            ChangeTracker.DetectChanges();
            var holder = ChangeTracker.Entries<PcaCode>();
            foreach (System.Data.Entity.Infrastructure.DbEntityEntry<PcaCode> entry in holder)
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
                hist.dbtable = "PcaCode";
                hist.change = "code: " + entry.Property(u => u.code).CurrentValue +
              "; startDate: " + entry.Property(u => u.startDate).CurrentValue +
              "; endDate: " + entry.Property(u => u.endDate).CurrentValue +
              "; description: " + entry.Property(u => u.description).CurrentValue +
              "; division: " + entry.Property(u => u.division).CurrentValue +
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