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
                ViewBag.userName = this.User.Identity.Name;
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
                return View("error");
            }
            else
            {
                return View("error");
            }
        }

        //
        // GET: /User/CheckForTimesheet
        //Function to create a new timesheet if one doesn't exist for the period
        public virtual int checkForTimesheet(Hours newhours)
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                Timesheet resulttimesheet = new Timesheet();
                DateTime startDay = newhours.timestamp.StartOfWeek(DayOfWeek.Sunday);

                //Check if there is a timesheet for the week that corresponds to newhours.timestamp
                var search = from m in TimesheetDB.TimesheetList
                             where m.worker.Contains(newhours.creator)
                             where m.periodStart <= newhours.timestamp
                             where m.periodEnd >= newhours.timestamp
                             select m;
                foreach (var item in search)
                {
                    //copy to placeholder timesheet so it can be edited
                    resulttimesheet = item;
                }
     
                //if there isn't a timesheet for the pay period, then create one
                //If there is a timesheet for the current pay period, don't do anything
                if (resulttimesheet.periodStart.CompareTo(startDay) != 0)
                {
                    Timesheet newTimesheet = new Timesheet();

                    //Set pay period to start on Sunday 12:00am
                    newTimesheet.periodStart = startDay;
                    newTimesheet.periodEnd = startDay.AddDays(7);
                    newTimesheet.worker = newhours.creator;
                    newTimesheet.approved = false;
                    newTimesheet.locked = false;
                    newTimesheet.submitted = false;

                    //add timesheet and save to the database
                    TimesheetDB.TimesheetList.Add(newTimesheet);
                    //TimesheetDB.Entry(newTimesheet).State = System.Data.EntityState.Added;
                    TimesheetDB.SaveChanges();

                    return 1;
                }
                return 1;
            }
            else
            {
                return 0;
            }
        }

        //
        //Function to retrieve a specified user's timesheet for specified date
        public Timesheet getTimesheet(string user, DateTime tsDate)
        {
            Timesheet resulttimesheet = new Timesheet();

            var search = from m in TimesheetDB.TimesheetList
                            where m.worker.Contains(user)
                            where m.periodStart <= tsDate
                            where m.periodEnd >= tsDate
                            select m;
            foreach (var item in search)
            {
                resulttimesheet = item;
            }
            return resulttimesheet;
        }

        //
        //Function to retrieve timesheet with specified unique id
        public Timesheet getTimesheetFromID(int id)
        {
            Timesheet resulttimesheet = new Timesheet();

            var search = from m in TimesheetDB.TimesheetList
                         where m.ID == id
                         select m;
            foreach (var item in search)
            {
                resulttimesheet = item;
            }
            return resulttimesheet;
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
                List<Hours> resultHours = new List<Hours>();

                //Select the timesheet from the previous pay period if it exists
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
                              where m.creator.Contains(user)
                              where m.timestamp >= previousTimesheet.periodStart
                              where m.timestamp <= previousTimesheet.periodEnd
                              select m;
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
                List<Task> resultTasks = new List<Task>();
                Timesheet timesheet = new Timesheet();
                string userName;

                if (User != null)
                {
                    userName = User.Identity.Name;
                }
                else
                {
                    userName = "";
                }
                //select all hours from current timesheet
                var searchHours = from m in HoursDB.HoursList
                                  where m.creator.Contains(userName)
                                  where m.timestamp >= startDay
                                  select m;
                foreach (var item in searchHours)
                {
                    //select task from each Hours entry
                    var searchTasks = from m in TaskDB.TaskList
                                      where m.ID == item.task
                                      select m;
                    resultTasks.AddRange(searchTasks);
                }
                var searchTimesheet = from m in TimesheetDB.TimesheetList
                                      where m.worker.Contains(userName)
                                      where m.periodStart <= DateTime.Now
                                      where m.periodEnd >= DateTime.Now
                                      select m;
                foreach (var item in searchTimesheet)
                {
                    timesheet = item;
                }
                ViewBag.taskList = resultTasks;
                ViewBag.timesheet = timesheet;
                return View(searchHours);
            }
            else
            {
                return View("error");
            }           
        }

        //
        // GET: /User/submitTimesheet
        //changes timesheet submitted status to true
        public virtual ActionResult submitTimesheet(int id)
        {
            if (id >= 0)
            {
                Authentication auth = new Authentication();
                if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
                {
                    Timesheet ts = new Timesheet();
                    ts = getTimesheetFromID(id);
                    ts.submitted = true;
                    TimesheetDB.Entry(ts).State = System.Data.EntityState.Modified;
                    //save changes to the database
                    TimesheetDB.SaveChanges();

                    return RedirectToAction("viewTimesheet/");
                }
                else
                {
                    return View("error");
                }
            }
            else
            {
                return View("error");
            }
        }

        //
        //Function to retrieve the status of an employees timesheet from the specified date
        public virtual string getTimesheetStatus(string userName, DateTime refDate)
        {
            string status = "";
            var tmptimesheet = new Timesheet();
            tmptimesheet = getTimesheet(userName, refDate);

            if (tmptimesheet.locked)
            {
                status = "locked";
            }
            else if (tmptimesheet.approved)
            {
                status = "approved";
            }
            else if (tmptimesheet.submitted)
            {
                status = "submitted";
            }
            else
            {
                status = "not submitted";
            }
            return status;
        }

        // 
        //Function that returns the current pay period as a string
        public virtual string getPayPeriod()
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                DateTime startDay = DateTime.Now.StartOfWeek(DayOfWeek.Sunday);
                DateTime endDay = startDay.AddDays(7);
                string payPeriod = startDay.ToShortDateString() + " - " + endDay.ToShortDateString();
                return payPeriod;
            }
            else
            {
                return "???";
            }
        }
    }
}
