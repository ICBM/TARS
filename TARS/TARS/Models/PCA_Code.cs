using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class PCA_Code
    {
        public int ID { get; set; } //DB iterator?
        public int code { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string description { get; set; }
    }

    public class PCA_CodeDBContext : DbContext
    {
        public DbSet<PCA_Code> PCA_CodeList { get; set; }
    }
}