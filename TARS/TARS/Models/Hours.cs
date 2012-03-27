﻿using System;
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
    public class Hours
    {
        public int ID { get; set; } //DB iterator
        public int workEffort { get; set; }
        public double hours { get; set; }
        public DateTime timestamp { get; set; }
        public bool approved { get; set; }
        public string description { get; set; } //User input
        public string creator { get; set; }
    }

    public class HoursDBContext : DbContext
    {
        public DbSet<Hours> HoursList { get; set; }
        protected HistoryDBContext HistDB = new HistoryDBContext();

        public int SaveChanges()
        {
            //call our code here
            var hist = new History();
            ChangeTracker.DetectChanges();
            var holder = ChangeTracker.Entries<Hours>();
            foreach (System.Data.Entity.Infrastructure.DbEntityEntry<Hours> entry in holder)
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
                hist.dbtable = "Hours";
                hist.change = "workEffort: " + entry.Property(u => u.workEffort).CurrentValue +
              "; hours: " + entry.Property(u => u.hours).CurrentValue +
              "; timestamp: " + entry.Property(u => u.timestamp).CurrentValue +
              "; approved: " + entry.Property(u => u.approved).CurrentValue +
              "; description: " + entry.Property(u => u.description).CurrentValue +
              "; creator: " + entry.Property(u => u.creator).CurrentValue;
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