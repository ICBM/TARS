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
        public int PCA { get; set; }
        public int WE { get; set; }
        public int hours { get; set; }
        public string hoursType { get; set; }
        public bool approved { get; set; }
        public DateTime timestamp { get; set; }
        public string description { get; set; }
    }

    public class HoursDBContext : DbContext
    {
        public DbSet<Hours> HoursList { get; set; }
    }
}