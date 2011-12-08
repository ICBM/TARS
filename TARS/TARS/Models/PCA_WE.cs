﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class PCA_WE
    {
        public int ID { get; set; } //DB iterator?
        public int PCA { get; set; }
        public int WE { get; set; }
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
                hist.change = "PCA: " + entry.Property(u => u.PCA).CurrentValue +
              "; WE: " + entry.Property(u => u.WE).CurrentValue;
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