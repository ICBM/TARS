using System;
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

        public int SaveChanges()
        {
            //call our code here
            return base.SaveChanges();
        }
    }
}