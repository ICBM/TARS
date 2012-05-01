using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class History
    {
        public int ID { get; set; } //DB iterator?
        public string username { get; set; }
        public string type { get; set; }
        public string change { get; set; }
        public string dbtable { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime timestamp { get; set; }
    }

    public class HistoryDBContext : DbContext
    {
        public DbSet<History> HistoryList { get; set; }

        public int SaveChanges()
        {
            //call our code here
            return base.SaveChanges();
        }
    }
}