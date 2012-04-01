﻿using System;
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
        protected TARSUserDBContext TARSUserDB = new TARSUserDBContext();
        protected DivisionsDBContext DivisionsDB = new DivisionsDBContext();
        protected EarningsCodesDBContext EarningsCodesDB = new EarningsCodesDBContext();
        protected PcaCodeDBContext PcaCodeDB = new PcaCodeDBContext();
        protected PCA_WEDBContext PCA_WEDB = new PCA_WEDBContext();
        
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
                return View("notLoggedIn");
            }
        }


        //
        // GET: /User/addHours
        //Adds hours to a work effort
        public virtual ActionResult addHours()
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                ViewBag.timesheetLockedFlag = isTimesheetLocked(User.Identity.Name, DateTime.Now);
                Authentication newAuth = new Authentication();
                bool adminFlag = newAuth.isAdmin(this);
                ViewBag.adminFlag = adminFlag;
                ViewBag.userName = User.Identity.Name;
                ViewBag.workEffortList = getActiveWorkEffortSelectList();
                return View();
            }
            else
            {
                return View("notLoggedIn");
            }
        }


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
                    if (newhours.creator == null)
                    {
                        return View("error");
                    }
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
                return View("notLoggedIn");
            }
        }


        //
        // GET: /User/CheckForTimesheet
        //Creates a new timesheet if one doesn't exist for the period
        public virtual int checkForTimesheet(Hours newhours)
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                Timesheet resulttimesheet = new Timesheet();
                DateTime startDay = newhours.timestamp.StartOfWeek(DayOfWeek.Sunday);

                //Check if there is a timesheet for the week that corresponds to newhours.timestamp
                var search = from m in TimesheetDB.TimesheetList
                             where (m.worker.CompareTo(newhours.creator) == 0)
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
        //Retrieves a specified user's timesheet for specified date
        public Timesheet getTimesheet(string user, DateTime tsDate)
        {
            Timesheet resulttimesheet = new Timesheet();

            var search = from m in TimesheetDB.TimesheetList
                            where (m.worker.CompareTo(user) == 0)
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
        //Retrieves timesheet with specified unique id
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
                var searchPermission = from m in TARSUserDB.TARSUserList
                             where (m.userName.CompareTo(this.User.Identity.Name) == 0) 
                             select m;
                foreach (var item in searchPermission)
                {
                    if (item.permission > 1)
                    {
                        ViewBag.managerFlag = true;
                    }
                }                
                return View(WorkEffortDB.WorkEffortList.ToList());
            }
            else
            {
                return View("notLoggedIn");
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
                var searchPermission = from m in TARSUserDB.TARSUserList
                             where (m.userName.CompareTo(this.User.Identity.Name) == 0)
                             select m;
                foreach (var item in searchPermission)
                {
                    if (item.permission > 1)
                    {
                        ViewBag.managerFlag = true;
                    }
                }
                ViewBag.pcaList = getWorkEffortPcaCodes(workeffort);
                ViewBag.WorkEffortID = workeffort.ID;
                return View(workeffort);
            }
            else
            {
                return View("notLoggedIn");
            }
        }


        // 
        // GET: /User/editHours
        //  - Edits a specified Hours entry
        public virtual ActionResult editHours(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                Hours hours = HoursDB.HoursList.Find(id);
                ViewBag.timesheetLockedFlag = isTimesheetLocked(User.Identity.Name, DateTime.Now);
                Authentication newAuth = new Authentication();
                bool adminFlag = newAuth.isAdmin(this);
                ViewBag.adminFlag = adminFlag;
                ViewBag.userName = User.Identity.Name;
                ViewBag.workEffortList = getActiveWorkEffortSelectList();
                return View(hours);
            }
            else
            {
                return View("notLoggedIn");
            }
        }


        //
        // POST: /User/editHours
        [HttpPost]
        public virtual ActionResult editHours(Hours tmpHours)
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                if (ModelState.IsValid)
                {
                    HoursDB.Entry(tmpHours).State = EntityState.Modified;
                    HoursDB.SaveChanges();
                }
                return RedirectToAction("viewTimesheet");
            }
            else
            {
                return View("notLoggedIn");
            }
        }


        //
        //Updates Locked status of timesheet that refHours is in, and returns timesheet status
        public bool isTimesheetLocked(string worker,DateTime refDate)
        {
            Timesheet tmpTimesheet = getTimesheet(worker, refDate);
            if (tmpTimesheet.locked == true)
            {
                return true;
            }
            if (tmpTimesheet.periodEnd < refDate.AddDays(-2))
            {
                //update locked status if end date was more than two days ago
                tmpTimesheet.locked = true;
                TimesheetDB.Entry(tmpTimesheet).State = EntityState.Modified;
                TimesheetDB.SaveChanges();
                return true;
            }
            return false;
        }


        // 
        // GET: /User/deleteHours
        //  - Deletes a specified Hours entry
        public virtual ActionResult deleteHours(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                Hours hours = HoursDB.HoursList.Find(id);
                HoursDB.Entry(hours).State = EntityState.Deleted;
                HoursDB.SaveChanges();
                return RedirectToAction("viewTimesheet");
            }
            else
            {
                return View("notLoggedIn");
            }
        }


        //
        // GET: /User/copyTimesheet
        //Duplicates hours from previous week 
        public virtual ActionResult copyTimesheet()
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                string userName = this.User.Identity.Name;
                Timesheet previousTimesheet = new Timesheet();
                DateTime dayFromPrevPeriod = DateTime.Now;
                dayFromPrevPeriod = dayFromPrevPeriod.AddDays(-7);
                List<Hours> resultHours = new List<Hours>();

                //Select the timesheet from the previous pay period if it exists
                var search = from m in TimesheetDB.TimesheetList
                             where (m.worker.CompareTo(userName) == 0)
                             where m.periodStart <= dayFromPrevPeriod
                             where m.periodEnd >= dayFromPrevPeriod
                             select m;
                foreach (var item in search)
                {
                    previousTimesheet = item;
                }
                //Iterate through each entry from previous week and duplicate it for this week
                var search2 = from m in HoursDB.HoursList
                              where (m.creator.CompareTo(userName) == 0)
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
                    //add new entry to Hours and History tables
                    addHours(copiedHours);
                }
                return RedirectToAction("viewTimesheet");
            }
            else
            {
                return View("notLoggedIn");
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
                return View("notLoggedIn");
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
                return View("notLoggedIn");
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
                Timesheet timesheet = new Timesheet();
                string userName;
                userName = User.Identity.Name;

                var searchTimesheet = from m in TimesheetDB.TimesheetList
                                        where (m.worker.CompareTo(userName) == 0)
                                        where m.periodStart <= DateTime.Now
                                        where m.periodEnd >= DateTime.Now
                                        select m;
                foreach (var item in searchTimesheet)
                {
                    timesheet = item;
                }
                ViewBag.timesheet = timesheet;

                //select all hours from current timesheet
                var searchHours = from m in HoursDB.HoursList
                                    where (m.creator.CompareTo(userName) == 0)
                                    where m.timestamp >= startDay
                                    select m;
                return View(searchHours);
            }
            else
            {
                return View("notLoggedIn");
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
                    return View("notLoggedIn");
                }
            }
            else
            {
                return View("error");
            }
        }


        //
        // GET: /User/unSubmitTimesheet
        //changes timesheet submitted status to true (only if it isn't in approved or locked status)
        public virtual ActionResult unSubmitTimesheet(int id)
        {
            if (id >= 0)
            {
                Authentication auth = new Authentication();
                if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
                {
                    Timesheet ts = new Timesheet();
                    ts = getTimesheetFromID(id);
                    //make sure the user is allowed to un-submit it
                    if ((ts.approved == false) && (ts.locked == false))
                    {
                        ts.submitted = false;
                        TimesheetDB.Entry(ts).State = System.Data.EntityState.Modified;
                        //save changes to the database
                        TimesheetDB.SaveChanges();
                    }
                    return RedirectToAction("viewTimesheet/");
                }
                else
                {
                    return View("notLoggedIn");
                }
            }
            else
            {
                return View("error");
            }
        }


        //
        //Retrieves the status of an employees timesheet from the specified date
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
        //Returns the current pay period as a string
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


        // 
        //Returns the IDHW Divisions as a list of strings
        public virtual List<string> getDivisions()
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                List<string> divList = new List<string>();
                var searchDivisions = from m in DivisionsDB.DivisionsList
                                      select m;
                foreach (var item in searchDivisions)
                {
                    divList.Add(item.divName);
                }
                return divList;
            }
            else
            {
                return null;
            }
        }


        // 
        //Returns list of PCA codes for the specified division
        public virtual List<string> getDivisionPcaCodes(string division)
        {
            List<string> pcaList = new List<string>();
            string tmpPca = "";
            var searchPca = from m in PcaCodeDB.PcaCodeList
                            where (m.division.CompareTo(division) == 0)
                            select m;
            foreach (var item in searchPca)
            {
                tmpPca = item.code.ToString();
                pcaList.Add(tmpPca);
            }
            return pcaList;
        }
 

        // 
        //Returns list of PCA codes for the specified division
        public virtual List<int> getWorkEffortPcaCodes(WorkEffort we)
        {
            List<int> pcaList = new List<int>();
            PcaCode tmpPca = new PcaCode();
            
            var searchPcaWe = from m in PCA_WEDB.PCA_WEList
                              where m.WE == we.ID
                              select m;
            foreach (var item in searchPcaWe)
            {
                tmpPca = PcaCodeDB.PcaCodeList.Find(item.PCA);
                pcaList.Add(tmpPca.code);
            }
            return pcaList;
        }


        // 
        //Returns Earnings Codes as a selection list that can be easily used in an Html DropDown
        public virtual List<SelectListItem> getEarningsCodeSelectList()
        {
            List<SelectListItem> earnCodesList = new List<SelectListItem>();
            var searchEarnCodes = from m in EarningsCodesDB.EarningsCodesList
                                    select m;
            foreach (var item in searchEarnCodes)
            {
                earnCodesList.Add(new SelectListItem
                {
                    Text = item.earningsCode + "  " + item.description,
                    Value = item.earningsCode
                });                    
            }
            return earnCodesList;
        }


        // 
        //Returns Work Efforts WITHIN THE USER'S DIVISION as a selection list
        public virtual List<SelectListItem> getActiveWorkEffortSelectList()
        {
            List<SelectListItem> effortList = new List<SelectListItem>();
            string division = getUserDivision();

            var searchEfforts = from m in WorkEffortDB.WorkEffortList
                                select m;
            var searchPca = from p in PcaCodeDB.PcaCodeList
                            where (p.division.CompareTo(division) == 0)
                            select p;

            //get all the work efforts in the user's division
            //(PCA codes and PCA_WE must be used to get all of the work efforts in the division)
            foreach (var we in searchEfforts)
            {
                if (we.hidden != true)
                {
                    foreach (var pca in searchPca)
                    {
                        var searchPcaWe = from p in PCA_WEDB.PCA_WEList
                                          where p.PCA == pca.ID
                                          where p.WE == we.ID
                                          select p;
                        foreach (var pca_we in searchPcaWe)
                        {
                            string tmpValue = we.ID.ToString();
                            effortList.Add(new SelectListItem
                            {
                                Text = we.description,
                                Value = tmpValue
                            });
                        }
                    }
                }
            }
            return effortList;
        }


        // 
        //Returns the description of a work effort as a string
        public virtual string getWeDescription(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                string weDescription = "";
                var searchWorkEfforts = from m in WorkEffortDB.WorkEffortList
                                        where m.ID == id
                                        select m;
                foreach (var item in searchWorkEfforts)
                {
                    weDescription = item.description;
                }
                return weDescription;
            }
            else
            {
                return null;
            }
        }


        // 
        //Returns the IDHW division that the user works for
        public virtual string getUserDivision()
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                string division = "";
                var searchUsers = from m in TARSUserDB.TARSUserList
                                        where (m.userName.CompareTo(User.Identity.Name) == 0)
                                        select m;
                foreach (var item in searchUsers)
                {
                    division = item.department;
                }
                return division;
            }
            else
            {
                return null;
            }
        }
    }
}
