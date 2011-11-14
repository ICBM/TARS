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
        protected WorkEffortDBContext WorkEffortdb = new WorkEffortDBContext();
        protected HoursDBContext Hoursdb = new HoursDBContext();


        //
        // GET: /User/
        public virtual ActionResult Index()
        {
            return View();
        }

        //
        // GET: /User/addHours
        public virtual ActionResult addHours(int id)
        {
            ViewBag.WorkEffortID = id;
            return View();
        }

        //
        // POST: /User/addHours
        [HttpPost]
        public virtual ActionResult addHours(Hours newhours)
        {
            if (ModelState.IsValid)
            {
                Hoursdb.HoursList.Add(newhours);
                Hoursdb.SaveChanges();
                return RedirectToAction("searchWorkEffort/");
            }
            return View(newhours);
        }

        //
        // GET: /User/searchWorkEffort
        public virtual ActionResult searchWorkEffort()
        {
            return View(WorkEffortdb.WorkEffortList.ToList());
        }

        //
        // GET: /User/viewWorkEffort
        public virtual ActionResult viewWorkEffort(int  id = 0)
        {
            WorkEffort workeffort = WorkEffortdb.WorkEffortList.Find(id);
            if (workeffort == null)
            {
                return HttpNotFound();
            }
            ViewBag.WorkEffortID = workeffort.ID;
            return View(workeffort);
        }

        //
        // GET: /User/viewHours
        public virtual ActionResult viewHours(string user="")
        {
            var search = from m in Hoursdb.HoursList
                         select m;
            if (!String.IsNullOrEmpty(user))
            {
                search = search.Where(s => s.creator.Contains(user));
            }

            return View(search);
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
