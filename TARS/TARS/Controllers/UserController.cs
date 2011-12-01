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
        protected WorkEffortDBContext WorkEffortDB = new WorkEffortDBContext();
        protected HoursDBContext HoursDB = new HoursDBContext();
        protected TaskDBContext TaskDB = new TaskDBContext();

        //
        // GET: /User/
        public virtual ActionResult Index()
        {
            //string user = "username"; //Need to swap this out
            string user;
            if(User != null)
            {
                user = User.Identity.Name;
            }
            else
            {
                user = "";
            }
            var searchHours = from m in HoursDB.HoursList
                         select m;
            List<Task> resultTasks = new List<Task>();
            if (!String.IsNullOrEmpty(user))
            {

                searchHours = searchHours.Where(s => s.creator.Contains(user));
            }
            foreach (var item in searchHours)
            {
                //searchTasks.Where(s => s.ID.Equals(1));
                var searchTasks = from m in TaskDB.TaskList
                                  where m.ID == item.task
                                  select m;
                resultTasks.AddRange(searchTasks);

            }

            ViewBag.taskList = resultTasks;
            //if (searchHours != null)
            {
                return View(searchHours);
            }
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
                HoursDB.HoursList.Add(newhours);
                HoursDB.SaveChanges();
                return RedirectToAction("searchWorkEffort/");
            }
            return View(newhours);
        }

        //
        // GET: /User/searchWorkEffort
        public virtual ActionResult searchWorkEffort()
        {
            return View(WorkEffortDB.WorkEffortList.ToList());
        }

        //
        // GET: /User/viewWorkEffort
        public virtual ActionResult viewWorkEffort(int id = 0)
        {
            WorkEffort workeffort = WorkEffortDB.WorkEffortList.Find(id);
            if (workeffort == null)
            {
                return HttpNotFound();
            }
            ViewBag.WorkEffortID = workeffort.ID;
            return View(workeffort);
        }

        //
        // GET: /User/viewHours
        public virtual ActionResult viewHours(string user = "")
        {
            var search = from m in HoursDB.HoursList
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
        public virtual ActionResult viewTimesheet()
        {

            string user = User.Identity.Name;
            var searchHours = from m in HoursDB.HoursList
                              select m;
            List<Task> resultTasks = new List<Task>();
            if (!String.IsNullOrEmpty(user))
            {

                searchHours = searchHours.Where(s => s.creator.Contains(user));
            }
            foreach (var item in searchHours)
            {
                //searchTasks.Where(s => s.ID.Equals(1));
                var searchTasks = from m in TaskDB.TaskList
                                  where m.ID == item.task
                                  select m;
                resultTasks.AddRange(searchTasks);

            }

            ViewBag.taskList = resultTasks;
            //if (searchHours != null)
            {
                return View(searchHours);
            }
        }
    }
}
