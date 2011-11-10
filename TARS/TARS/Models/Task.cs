using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TARS.Models
{
    public class Task
    {    
        public int ID { get; set; } //DB iterator
        public int WE { get; set; }
        public string title { get; set; }
    }

    public class TaskDBContext : DbContext
    {
        public DbSet<Task> TaskList { get; set; }

        public int SaveChanges()
        {
            //call our code here
            return base.SaveChanges();
        }
    }
}
