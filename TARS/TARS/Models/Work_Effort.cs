using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class Work_Effort
    {
        public int ID { get; set; } //DB iterator?
        public int code { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string description { get; set; }
    }

    public class Work_EffortDBContext : DbContext
    {
        public DbSet<Work_Effort> Work_EffortList { get; set; }
    }
}