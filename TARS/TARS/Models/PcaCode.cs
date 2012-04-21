using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using TARS.Helpers;
using TARS.Models;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class PcaCode
    {    
        public int ID { get; set; } //DB iterator
        public string description { get; set; }
        public string division { get; set; }

        [DataType(DataType.Date)]
        public DateTime startDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime endDate { get; set; }
        public PcaCode()
        {
            startDate = DateTime.Now;
            endDate = DateTime.Now;
        }

        [Required]
        [Range(10000, 99999, ErrorMessage = "PCA Code must be a 5 digit number")]
        public int code { get; set; }
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
                hist.change = "pcaCode: " + entry.Property(u => u.code).CurrentValue +
                              "; description: " + entry.Property(u => u.description).CurrentValue +
                              "; division: " + entry.Property(u => u.division).CurrentValue +
                              "; startDate: " + entry.Property(u => u.startDate).CurrentValue.ToShortDateString() +
                              "; endDate: " + entry.Property(u => u.endDate).CurrentValue.ToShortDateString();
            }

            hist.username = System.Web.HttpContext.Current.User.Identity.Name;
            hist.timestamp = System.DateTime.Now;
            HistDB.HistoryList.Add(hist);
            HistDB.SaveChanges();

            return base.SaveChanges();
        }
    }
}