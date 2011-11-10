using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class Hours
    {
        public int ID { get; set; } //DB iterator?
        //public int pca { get; set; }
        public int we { get; set; }
        public int task { get; set; }
        public float hours { get; set; }
        public string hoursType { get; set; } //Earnings code || Might be ignored and dealt with by PCA!
        public bool approved { get; set; }
        public DateTime timestamp { get; set; }
        public string description { get; set; } //User input
    }

    public class HoursDBContext : DbContext
    {
        public DbSet<Hours> HoursList { get; set; }

        public int SaveChanges()
        {
            //call our code here
            return base.SaveChanges();
        }
    }
}