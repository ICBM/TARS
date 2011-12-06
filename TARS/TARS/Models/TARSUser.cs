using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//Necessary for DB calls.
using System.Data.Entity;

namespace TARS.Models
{
    public class TARSUser
    {
        public int ID { get; set; } //DB iterator.
        public string un { get; set; } //This might be changed to uid if Active Directory ends up playing nicer with that.
        public int permission { get; set; } //1 User   2 Manager   3 Admin
    }

    public class TARSUserDBContext : DbContext
    {
        public DbSet<TARSUser> TARSUserList { get; set; }

        public int SaveChanges()
        {
            //call our code here
            return base.SaveChanges();
        }
    }
}