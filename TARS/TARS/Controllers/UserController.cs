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
        protected TimesheetDBContext TimesheetDB = new TimesheetDBContext();
        protected TaskDBContext TaskDB = new TaskDBContext();
        protected TARSUserDBContext TARSUserDB = new TARSUserDBContext();
        
        //
        // GET: /User/
        //Index view, redirects to viewTimesheet function
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
        //Function to add hours to a workeffort with index id
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
/*        public virtual ActionResult addHours()
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                return View();
            }
            else
            {
                return View("error");
            }
        }*/

        //
        // POST: /User/addHours
        //Takes filled form and adds it to database
        [HttpPost]
        public virtual ActionResult addHours(Hours newhours)
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                if (ModelState.IsValid)
                {
                    checkForTimesheet(newhours);
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
        // GET: /User/CheckForTimesheet
        //Function to create a new timesheet if one doesn't exist for the period
        public virtual ActionResult checkForTimesheet(Hours newhours)
        {
newhours.creator = "zeke";
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                Timesheet resulttimesheet = new Timesheet();
                Timesheet previoustimesheet = new Timesheet();
                var search = from m in TimesheetDB.TimesheetList
                             select m;
                var search2 = search;   //This will be used if the first search doesn't find a match

                //Find timesheet for the week that corresponds to newhours.timestamp
                search = search.Where(s => s.worker.Contains(newhours.creator));
                search = search.Where(s => (s.periodStart <= newhours.timestamp));
                search = search.Where(s => (s.periodEnd >= newhours.timestamp));
                foreach (var item in search)
                {
                    resulttimesheet = item;
                }
                //If there isn't a timesheet for the newhours, then create one
                if (resulttimesheet.periodStart == null)
                {
                    DateTime dayFromPrevPeriod = newhours.timestamp;
                    dayFromPrevPeriod = dayFromPrevPeriod.AddDays(-7);

                    //Find timesheet for the week before so we can use the end date as the new start date
                    search2 = search2.Where(s => s.worker.Contains(newhours.creator));
                    search2 = search2.Where(s => (s.periodStart <= dayFromPrevPeriod));
                    search2 = search2.Where(s => (s.periodEnd >= dayFromPrevPeriod));
                    foreach (var item in search2)
                    {
                        previoustimesheet = item;
                    }
                    //If there isn't a timesheet from the week before, then use 
                    if (previoustimesheet.periodStart == null)
                    {
                        //Set pay period to start on Sunday 12:00am
                        DateTime startDay = newhours.timestamp.StartOfWeek(DayOfWeek.Sunday);
                        resulttimesheet.periodStart = startDay;
                        resulttimesheet.periodEnd = startDay.AddDays(7);
                    }
                    else
                    {
                        resulttimesheet.periodStart = previoustimesheet.periodEnd;
                        resulttimesheet.periodEnd = resulttimesheet.periodStart.AddDays(7);
                    }
                }
                else
                {
                    //Set pay period to start on Sunday 12:00am
                    DateTime startDay = newhours.timestamp.StartOfWeek(DayOfWeek.Sunday);
                    resulttimesheet.periodStart = startDay;
                    resulttimesheet.periodEnd = startDay.AddDays(7);
                }
                resulttimesheet.worker = newhours.creator;
                resulttimesheet.approved = false;
                resulttimesheet.locked = false;
                TimesheetDB.TimesheetList.Add(resulttimesheet);
                //add timesheet to the database
                TimesheetDB.SaveChanges();

                return RedirectToAction("viewTimesheet/");
            }
            else
            {
                return RedirectToAction("viewTimesheet/");
            }
        }

        //
        // GET: /User/searchWorkEffort
        //Lists out all workefforts in the database
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
        //View the details of a workeffort
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
        //Look at a user's submitted hours.
        //Can take a string as an argument with ?user="userName" in the URL right now
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
        // GET: /User/repeatTimesheet
        //Function to duplicate hours from previous week 
        public virtual ActionResult repeatTimesheet(string user = "")
        {
user = "zeke";
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                var search = from m in TimesheetDB.TimesheetList
                              select m;
                Timesheet previousTimesheet = new Timesheet();
                DateTime dayFromPrevPeriod = DateTime.Now;
                dayFromPrevPeriod = dayFromPrevPeriod.AddDays(-7);
                search = search.Where(s => s.worker.Contains(user));
                search = search.Where(s => (s.periodStart <= dayFromPrevPeriod));
                search = search.Where(s => (s.periodEnd >= dayFromPrevPeriod));
                foreach (var item in search)
                {
                    previousTimesheet = item;
                }

                //Iterate through each entry from previous week and duplicate it for this week
                var search2 = from m in HoursDB.HoursList
                             select m;
                List<Hours> resultHours = new List<Hours>();
                if (!String.IsNullOrEmpty(user))
                {
                    search2 = search2.Where(s => s.creator.Contains(user));
                    search2 = search2.Where(s => s.timestamp >= previousTimesheet.periodStart);
                    search2 = search2.Where(s => s.timestamp <= previousTimesheet.periodEnd);
                }
                foreach (var item in search2)
                {
                    resultHours.Add(item);
                }
                foreach (var copiedHours in resultHours)
                {
                    copiedHours.timestamp = copiedHours.timestamp.AddDays(7);
                    copiedHours.approved = false;
                    //add new entry to Hours and History tables
                    addHours(copiedHours);
                }
                return RedirectToAction("viewTimesheet/");
            }
            else
            {
                return View("error");
            }
        }

        //
        // GET: /User/storeFile
        //Unimplemented
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
        //Unimplemented
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

        //
        // GET: /User/viewTimesheet
        //Gets a list of the current user's hours and associated tasks
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
