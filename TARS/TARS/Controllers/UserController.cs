using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TARS.Models;
using TARS.Helpers;

namespace TARS.Controllers
{
    public class UserController : Controller
    {
        protected WorkEffortDBContext WorkEffortDB = new WorkEffortDBContext();
        protected HoursDBContext HoursDB = new HoursDBContext();
        protected TaskDBContext TaskDB = new TaskDBContext();
        protected TARSUserDBContext TARSUserDB = new TARSUserDBContext();

        //
        // GET: /User/
        public virtual ActionResult Index()
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth )
            {
                return Redirect("/TARS/User/viewTimesheet/");
            }
            else
            {
                return View("error");
            }
        }
        //
        // GET: /User/addHours
        public virtual ActionResult addHours(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                ViewBag.WorkEffortID = id;
                return View();
            }
            else
            {
                return View("error");
            }
        }

        //
        // POST: /User/addHours
        [HttpPost]
        public virtual ActionResult addHours(Hours newhours)
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                if (ModelState.IsValid)
                {
                    HoursDB.HoursList.Add(newhours);
                    HoursDB.SaveChanges();
                    return RedirectToAction("viewTimesheet/");
                }
                return View(newhours);
            }
            else
            {
                return View("error");
            }
        }

        //
        // GET: /User/searchWorkEffort
        public virtual ActionResult searchWorkEffort()
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                return View(WorkEffortDB.WorkEffortList.ToList());
            }
            else
            {
                return View("error");
            }
        }

        //
        // GET: /User/viewWorkEffort
        public virtual ActionResult viewWorkEffort(int id = 0)
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                WorkEffort workeffort = WorkEffortDB.WorkEffortList.Find(id);
                if (workeffort == null)
                {
                    return HttpNotFound();
                }
                ViewBag.WorkEffortID = workeffort.ID;
                return View(workeffort);
            }
            else
            {
                return View("error");
            }
        }

        //
        // GET: /User/viewHours
        public virtual ActionResult viewHours(string user = "")
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                var search = from m in HoursDB.HoursList
                             select m;
                if (!String.IsNullOrEmpty(user))
                {
                    search = search.Where(s => s.creator.Contains(user));
                }

                return View(search);
            }
            else
            {
                return View("error");
            } 
        }

        //
        // GET: /User/storeFile
        public virtual ActionResult storeFile()
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                return null;
            }
            else
            {
                return View("error");
            }
        }

        //
        // GET: /User/viewHistory
        public virtual ActionResult viewHistory()
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                return null;
            }
            else
            {
                return View("error");
            }
        }

        public virtual ActionResult viewTimesheet()
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                string user;
                if (User != null)
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
            else
            {
                return View("error");
            }
            
        }
    }
}
