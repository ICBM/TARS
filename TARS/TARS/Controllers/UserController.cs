using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TARS.Models;

namespace TARS.Controllers
{
    public class UserController : Controller
    {
        protected WorkEffortDBContext db = new WorkEffortDBContext();

        //
        // GET: /User/
        public virtual ActionResult Index()
        {
            return View();
        }

        //
        // GET: /User/addHours
        public virtual ActionResult addHours()
        {
            return View();
        }

        //
        // POST: /User/addHours
        [HttpPost]
        public virtual ActionResult addHours(Hours hours)
        {
            return View();
        }

        //
        // GET: /User/searchWorkEffort
        public virtual ActionResult searchWorkEffort()
        {
            return View(db.WorkEffortList.ToList());
        }

        //
        // GET: /User/viewWorkEffort
        public virtual ActionResult viewWorkEffort(int  id)
        {
            WorkEffort workeffort = db.WorkEffortList.Find(id);
            return View(workeffort);
        }

        //
        // GET: /User/viewHours
        public virtual ActionResult viewHours()
        {
            return null;
        }

        //
        // GET: /User/storeFile
        public virtual ActionResult storeFile()
        {
            return null;
        }

        //
        // GET: /User/viewHistory
        public virtual ActionResult viewHistory()
        {
            return null;
        }
    }
}
