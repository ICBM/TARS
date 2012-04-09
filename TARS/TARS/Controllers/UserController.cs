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
                return RedirectToAction("viewTimesheet", new { tsDate = DateTime.Now });
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
                string division = TempData["division"].ToString();
                ViewBag.division = division;
                ViewBag.workEffortList = getVisibleWorkEffortSelectList(division);
                Authentication newAuth = new Authentication();
                bool adminFlag = newAuth.isAdmin(this);
                ViewBag.adminFlag = adminFlag;
                ViewBag.userName = User.Identity.Name;
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
        public virtual ActionResult addHours(Hours newhours, string division = null)
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                if (ModelState.IsValid)
                {
                    WorkEffort tmpWe = WorkEffortDB.WorkEffortList.Find(newhours.workEffortID);
                    bool tsLockedFlag = isTimesheetLocked(User.Identity.Name, newhours.timestamp);

                    /* check to make sure that the new hours are within the work effort's time bounds 
                       and the timesheet isn't locked
                    */
                    if ((newhours.timestamp < tmpWe.startDate) || 
                        (newhours.timestamp > tmpWe.endDate) || 
                        (tsLockedFlag == true))
                    {
                        ViewBag.invalidTimestamp = true;
                        ViewBag.timesheetLockedFlag = tsLockedFlag;
                        Authentication newAuth = new Authentication();
                        bool adminFlag = newAuth.isAdmin(this);
                        ViewBag.adminFlag = adminFlag;
                        ViewBag.userName = User.Identity.Name;
                        ViewBag.division = division;
                        ViewBag.workEffortList = getVisibleWorkEffortSelectList(division);
                        return View(newhours);
                    }
                    //make sure that a timesheet exists for the period hours are being added to
                    checkForTimesheet(newhours);
  
                    //add and save new hours
                    HoursDB.HoursList.Add(newhours);
                    HoursDB.SaveChanges();
                    return RedirectToAction("viewTimesheet", new { tsDate = newhours.timestamp });
                }
                return View("error");
            }
            else
            {
                return View("notLoggedIn");
            }
        }


        //
        //GET: /User/selectDivisionToAddHours
        //
        public virtual ActionResult selectDivisionToAddHours()
        {
            List<string> divisions = getDivisionsList();
            string userDivision = getUserDivision();
            var divisionList = new SelectList(divisions, userDivision);
            ViewBag.divisionList = divisionList;
            return View() ;
        }


        //
        //POST: /User/selectDivisionToAddHours
        //Passes the selected division to addHours so the appropriate Work Efforts can be displayed
        [HttpPost]
        public virtual ActionResult selectDivisionToAddHours(string division)
        {
            TempData["division"] = division;
            return RedirectToAction("addHours");
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
                    createCurrentTimesheet(User.Identity.Name);
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
        //Creates an empty timesheet for the specified user
        public void createCurrentTimesheet(string userName)
        {
            Timesheet newTimesheet = new Timesheet();
            DateTime startDay = DateTime.Now.StartOfWeek(DayOfWeek.Sunday);
            //Set pay period to start on Sunday 12:00am
            newTimesheet.periodStart = startDay;
            newTimesheet.periodEnd = startDay.AddDays(7);
            newTimesheet.worker = userName;
            newTimesheet.approved = false;
            newTimesheet.locked = false;
            newTimesheet.submitted = false;

            //add timesheet and save to the database
            TimesheetDB.TimesheetList.Add(newTimesheet);
            TimesheetDB.Entry(newTimesheet).State = System.Data.EntityState.Added;
            TimesheetDB.SaveChanges();
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
                return resulttimesheet;
            }
            return null;
        }


        //
        // GET: /User/searchWorkEffort
        //Lists out all workefforts in the database
        public virtual ActionResult searchWorkEffort()
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                var workEffortList = WorkEffortDB.WorkEffortList.ToList();
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
                //create a list of lists (each work effort will have a list of PCA codes)
                ViewBag.pcaListOfLists = new List<List<int>>();
                foreach (var item in workEffortList)
                {
                    ViewBag.pcaListOfLists.Add(getWorkEffortPcaCodes(item));
                }
                return View(workEffortList);
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
        //  - Edits a specified Hours entry for the logged in user
        public virtual ActionResult editHours(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                Hours hours = HoursDB.HoursList.Find(id);
                ViewBag.timesheetLockedFlag = isTimesheetLocked(hours.creator, hours.timestamp);
                Authentication newAuth = new Authentication();
                bool adminFlag = newAuth.isAdmin(this);
                ViewBag.adminFlag = adminFlag;
                ViewBag.userName = User.Identity.Name;
                ViewBag.workEffort = WorkEffortDB.WorkEffortList.Find(hours.workEffortID);
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
                return RedirectToAction("viewTimesheet", new {tsDate=tmpHours.timestamp});
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
            if (tmpTimesheet != null)
            {
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
                return RedirectToAction("viewTimesheet", new { tsDate = hours.timestamp });
            }
            else
            {
                return View("notLoggedIn");
            }
        }


        //
        // GET: /User/copyTimesheet
        //Duplicates timesheet from previous week (but hours worked are set to zero)
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
                    addHours(copiedHours);
                }
                return RedirectToAction("viewTimesheet", new { tsDate = dayFromPrevPeriod });
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
        //Gets the current user's hours for the time period that tsDate falls within
        public virtual ActionResult viewTimesheet(DateTime tsDate)
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                string userName = User.Identity.Name;
                Timesheet timesheet = getTimesheet(userName, tsDate);
                Timesheet prevTimesheet = getTimesheet(userName, tsDate.AddDays(-7));

                ViewBag.timesheet = timesheet;
                if (prevTimesheet == null)
                {
                    ViewBag.noPreviousTimesheet = true;
                }

                //select all hours from the timesheet
                var searchHours = from m in HoursDB.HoursList
                                    where (m.creator.CompareTo(userName) == 0)
                                    where m.timestamp >= timesheet.periodStart
                                    where m.timestamp <= timesheet.periodEnd
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
                    ts = TimesheetDB.TimesheetList.Find(id);
                    ts.submitted = true;
                    TimesheetDB.Entry(ts).State = System.Data.EntityState.Modified;
                    //save changes to the database
                    TimesheetDB.SaveChanges();

                    return RedirectToAction("viewTimesheet", new { tsDate = ts.periodStart });
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
            if (tmptimesheet != null)
            {
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
        public virtual List<string> getDivisionsList()
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
        //Returns active Work Efforts WITHIN THE SPECIFIED DIVISION as a selection list
        public virtual List<SelectListItem> getVisibleWorkEffortSelectList(string division)
        {
            List<SelectListItem> effortList = new List<SelectListItem>();
            PcaCode tmpPca = new PcaCode();
            string tmpValue = "";

            var searchEfforts = from m in WorkEffortDB.WorkEffortList
                                select m;

            //narrow down to work efforts in the specified division
            //(PCA codes and PCA_WE must be used to get all of the work efforts in the division)
            foreach (var we in searchEfforts)
            {
                if (we.hidden != true)
                {
                    var searchPcaWe = from p in PCA_WEDB.PCA_WEList
                                        where p.WE == we.ID
                                        select p;
                    foreach (var pca_we in searchPcaWe)
                    {
                        tmpPca = PcaCodeDB.PcaCodeList.Find(pca_we.PCA);
                        //if the PCA is in the user's division, then add the Work Effort to the list
                        if (tmpPca.division.CompareTo(division) == 0)
                        {
                            tmpValue = we.ID.ToString();
                            effortList.Add(new SelectListItem
                            {
                                Text = we.description,
                                Value = tmpValue
                            });
                            break;
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
        //Returns work effort's time boundaries as a string
        //(note: it's called from addHours View)
        public string getWeTimeBoundsString(int id)
        {
            WorkEffort we = WorkEffortDB.WorkEffortList.Find(id);
            string bounds = we.startDate.ToShortDateString() + " - " + we.endDate.ToShortDateString();
            return bounds;
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
                    division = item.company;
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
