using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class PcaCode
    {    
        public int ID { get; set; } //DB iterator?
        public int code { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string description { get; set; }
        public bool active { get; set; }
    }

    public class PcaCodeDBContext : DbContext
    {
        public DbSet<PcaCode> PcaCodeList { get; set; }

        public int SaveChanges()
        {
            //call our code here
            return base.SaveChanges();
        }
    }
}