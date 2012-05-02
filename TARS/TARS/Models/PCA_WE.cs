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
    public class PCA_WE
    {
        public int ID { get; set; } //DB iterator
        public int PCA { get; set; }
        public int WE { get; set; }
        public bool active { get; set; }
        [DataType(DataType.Date)]
        public DateTime? associationStartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? associationEndDate { get; set; }
    }

    public class PCA_WEDBContext : DbContext
    {
        public DbSet<PCA_WE> PCA_WEList { get; set; }
        protected HistoryDBContext HistDB = new HistoryDBContext();

        public int SaveChanges()
        {
            //call our code here
            var hist = new History();
            ChangeTracker.DetectChanges();
            var holder = ChangeTracker.Entries<PCA_WE>();
            foreach (System.Data.Entity.Infrastructure.DbEntityEntry<PCA_WE> entry in holder)
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
                hist.dbtable = "PCA_WE";
                hist.change = "pcaID: " + entry.Property(u => u.PCA).CurrentValue +
                              "; workEffortID: " + entry.Property(u => u.WE).CurrentValue +
                              "; associationStart: " + entry.Property(u => u.associationStartDate).CurrentValue +
                              "; associationEnd: " + entry.Property(u => u.associationEndDate).CurrentValue +
                              "; active: " + entry.Property(u => u.WE).CurrentValue;
            }

            hist.username = System.Web.HttpContext.Current.User.Identity.Name;
            hist.timestamp = System.DateTime.Now;
            HistDB.HistoryList.Add(hist);
            HistDB.SaveChanges();

            return base.SaveChanges();
        }
    }
}