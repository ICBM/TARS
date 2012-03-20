using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class TARSUser
    {
        public int ID { get; set; } //DB iterator.
        public string un { get; set; } //This might be changed to uid if Active Directory ends up playing nicer with that.
        public int permission { get; set; } //1 User   2 Manager   3 Admin 
        public bool costAllocated { get; set; }
        public DateTime contractorStart { get; set; }
        public string contractorName { get; set; }       
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
                hist.dbtable = "TARSUser";
                hist.change = "un: " + entry.Property(u => u.un).CurrentValue +
              "; task: " + entry.Property(u => u.permission).CurrentValue;
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