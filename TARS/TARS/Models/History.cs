using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class History
    {
        public int ID { get; set; } //DB iterator?
        public int PCA { get; set; }
        public int WE { get; set; }
        public string creator { get; set; }
        public DateTime timestamp { get; set; }
        public int hours { get; set; }
        public string hoursType { get; set; }
        public string description { get; set; }
    }

    public class HistoryDBContext : DbContext
    {
        public DbSet<History> HistoryList { get; set; }
    }
}