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
                    //check to see if a timesheet exists for the period hours are being added to
                    checkForTimesheet(newhours);
                    //add and save new hours
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
                search = from m in search
                         where m.worker.Contains(newhours.creator)
                         where m.periodStart <= newhours.timestamp
                         where m.periodEnd >= newhours.timestamp
                         select m;
                foreach (var item in search)
                {
                    //this is necessary so the timesheet can be edited
                    resulttimesheet = item;
                }
                //If there isn't a timesheet for the newhours, then create one
                if (resulttimesheet.periodStart == null)
                {
                    DateTime dayFromPrevPeriod = newhours.timestamp;
                    dayFromPrevPeriod = dayFromPrevPeriod.AddDays(-7);

                    //Find timesheet for the week before so we can use the end date as the new start date
                    search2 = from m in search2
                              where m.worker.Contains(newhours.creator)
                              where m.periodStart <= dayFromPrevPeriod
                              where m.periodEnd >= dayFromPrevPeriod
                              select m;
                    foreach (var item in search2)
                    {
                        previoustimesheet = item;
                    }
                    //If there isn't a timesheet from the week before 
                    if (previoustimesheet.periodStart == null)
                    {
                        //Set pay period to start on Sunday 12:00am
                        DateTime startDay = newhours.timestamp.StartOfWeek(DayOfWeek.Sunday);
                        resulttimesheet.periodStart = startDay;
                        resulttimesheet.periodEnd = startDay.AddDays(7);
                    }
                    else
                    {
                        //Set pay period to start where the previous period ended
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
                resulttimesheet.submitted = false;
                //add timesheet and save to the database
                TimesheetDB.TimesheetList.Add(resulttimesheet);
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
        // GET: /User/copyTimesheet
        //Function to duplicate hours from previous week 
        public virtual ActionResult copyTimesheet(string user)
        {
user = "zeke";
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                Timesheet previousTimesheet = new Timesheet();
                DateTime dayFromPrevPeriod = DateTime.Now;
                dayFromPrevPeriod = dayFromPrevPeriod.AddDays(-7);

                var search = from m in TimesheetDB.TimesheetList
                             where m.worker.Contains(user)
                             where m.periodStart <= dayFromPrevPeriod
                             where m.periodEnd >= dayFromPrevPeriod
                             select m;
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
                    search2 = from m in search2
                              where m.creator.Contains(user)
                              where m.timestamp >= previousTimesheet.periodStart
                              where m.timestamp <= previousTimesheet.periodEnd
                              select m;
                }
                foreach (var item in search2)
                {
                    resultHours.Add(item);
                }
                foreach (var copiedHours in resultHours)
                {
                    copiedHours.hours = 0;
                    copiedHours.timestamp = DateTime.Now;
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
                DateTime startDay = DateTime.Now.StartOfWeek(DayOfWeek.Sunday);
                string user;
                if (User != null)
                {
                    user = User.Identity.Name;
                }
                else
                {
                    user = "";
                }
                //select all hours from current timesheet
                var searchHours = from m in HoursDB.HoursList
                                  where m.creator.Contains(user)
                                  where m.timestamp >= startDay
                                  select m;
                List<Task> resultTasks = new List<Task>();
                foreach (var item in searchHours)
                {
                    //select task from each Hours entry
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

        //
        // GET: /User/submitTimesheet
        //changes timesheet status to true so it will show up in the manager's list of timesheets to approve
        public virtual ActionResult submitTimesheet()
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
//need to fix this to be actual user
var user = "zeke";
                Timesheet tmptimesheet = new Timesheet();
                DateTime startDay = DateTime.Now.StartOfWeek(DayOfWeek.Sunday);
                //select current timesheet
                var search = from m in TimesheetDB.TimesheetList
                             where m.worker.Contains(user)
                             where m.periodStart >= startDay
                             select m; 
                foreach (var item in search)
                {
                    tmptimesheet = item;
                }
                tmptimesheet.submitted = true;
                TimesheetDB.TimesheetList.Add(tmptimesheet);
                TimesheetDB.Entry(tmptimesheet).State = System.Data.EntityState.Modified;
                //save changes to the database
                TimesheetDB.SaveChanges();

                return RedirectToAction("viewTimesheet/");
            }
            else
            {
                return View("error");
            }
        }
    }
}
