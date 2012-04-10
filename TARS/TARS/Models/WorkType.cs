using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TARS.Helpers;
using TARS.Models;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class WorkType
    {
        public int ID { get; set; }
        public int WE { get; set; }
        public string description { get; set; }
    }

    public class WorkTypeDBContext : DbContext
    {
        public DbSet<WorkType> WorkTypeList { get; set; }
        protected HistoryDBContext HistDB = new HistoryDBContext();

        public int SaveChanges()
        {
            //call our code here
            var hist = new History();
            ChangeTracker.DetectChanges();
            var holder = ChangeTracker.Entries<WorkType>();
            foreach (System.Data.Entity.Infrastructure.DbEntityEntry<WorkType> entry in holder)
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
                hist.dbtable = "Work Type";
                hist.change = "WE: " + entry.Property(u => u.WE).CurrentValue +
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