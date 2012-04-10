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
        protected WorkTypeDBContext WorkTypeDB = new WorkTypeDBContext();
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
                Authentication newAuth = new Authentication();
                bool adminFlag = newAuth.isAdmin(this);
                ViewBag.adminFlag = adminFlag;
                ViewBag.userName = User.Identity.Name;
                ViewBag.divisionList = getDivisionSelectList();
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
                    WorkEffort tmpWe = WorkEffortDB.WorkEffortList.Find(newhours.workEffortID);
                    bool tsLockedFlag = isTimesheetLocked(User.Identity.Name, newhours.timestamp);
                    Authentication newAuth = new Authentication();
                    bool adminFlag = newAuth.isAdmin(this);

                    //make sure that the new hours are within the work effort's time bounds 
                    if ((newhours.timestamp < tmpWe.startDate) || (newhours.timestamp > tmpWe.endDate))
                    {
                        ViewBag.notWithinWeBounds = true;
                        ViewBag.timesheetLockedFlag = tsLockedFlag;
                        ViewBag.adminFlag = adminFlag;
                        ViewBag.userName = User.Identity.Name;
                        ViewBag.divisionList = getDivisionSelectList();
                        return View(newhours);
                    }
                    //make sure that only Admin can add hours to locked timesheets
                    if ((tsLockedFlag == true) && (adminFlag == false))
                    {
                        ViewBag.timesheetLockedFlag = true;
                        ViewBag.adminFlag = false;
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
        // GET: /User/CheckForTimesheet
        //Creates a new timesheet if one doesn't exist for the period
        public void checkForTimesheet(Hours newhours)
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                Timesheet resulttimesheet = new Timesheet();
                DateTime startDay = newhours.timestamp.StartOfWeek(DayOfWeek.Sunday);

                //Check if there is a timesheet for the week that corresponds to newhours.timestamp
                var searchTs = from m in TimesheetDB.TimesheetList
                             where (m.worker.CompareTo(newhours.creator) == 0)
                             where m.periodStart <= newhours.timestamp
                             where m.periodEnd >= newhours.timestamp
                             select m;
                foreach (var item in searchTs)
                {
                    resulttimesheet = item;
                }
     
                //if there isn't a timesheet for the pay period, then create one
                //If there is a timesheet for the current pay period, don't do anything
                if (resulttimesheet.periodStart.CompareTo(startDay) != 0)
                {
                    createCurrentTimesheet(User.Identity.Name);
                    return;
                }
                return;
            }
            else
            {
                return;
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

            var searchTs = from m in TimesheetDB.TimesheetList
                            where (m.worker.CompareTo(user) == 0)
                            where m.periodStart <= tsDate
                            where m.periodEnd >= tsDate
                            select m;
            foreach (var item in searchTs)
            {
                if (item != null)
                {
                    resulttimesheet = item;
                    return resulttimesheet;
                }
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
                Authentication auth2 = new Authentication();
                if (auth2.isManager(this))
                {
                    ViewBag.managerFlag = true;
                }

                var workEffortList = WorkEffortDB.WorkEffortList.ToList();
                //create a list of lists for pca codes and work types
                //(each work effort will have a list of PCA codes and a list work types)
                ViewBag.pcaListOfLists = new List<List<int>>();
                ViewBag.workTypesListOfLists = new List<List<string>>();
                foreach (var item in workEffortList)
                {
                    ViewBag.pcaListOfLists.Add(getWorkEffortPcaCodes(item));
                    ViewBag.workTypesListOfLists.Add(getWorkEffortWorkTypeList(item));
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
        public virtual ActionResult viewWorkEffort(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                Authentication newAuth = new Authentication();
                if (newAuth.isManager(this))
                {
                    ViewBag.managerFlag = true;
                }

                WorkEffort workeffort = WorkEffortDB.WorkEffortList.Find(id);
                if (workeffort == null)
                {
                    return HttpNotFound();
                }
                ViewBag.pcaList = getWorkEffortPcaCodes(workeffort);
                ViewBag.workTypeList = getWorkEffortWorkTypeList(workeffort);
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
                WorkEffort we = WorkEffortDB.WorkEffortList.Find(hours.workEffortID);
                ViewBag.timesheetLockedFlag = isTimesheetLocked(hours.creator, hours.timestamp);
                Authentication newAuth = new Authentication();
                bool adminFlag = newAuth.isAdmin(this);
                ViewBag.adminFlag = adminFlag;
                ViewBag.userName = User.Identity.Name;
                ViewBag.workEffort = we;
                ViewBag.workTypeList = getWorkEffortWorkTypeList(we);
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
                string userName = User.Identity.Name;
                Timesheet previousTimesheet = new Timesheet();
                DateTime dayFromPrevPeriod = DateTime.Now;
                dayFromPrevPeriod = dayFromPrevPeriod.AddDays(-7);
                List<Hours> resultHours = new List<Hours>();

                //Select the timesheet from the previous pay period if it exists
                var searchTs = from m in TimesheetDB.TimesheetList
                               where (m.worker.CompareTo(userName) == 0)
                               where m.periodStart <= dayFromPrevPeriod
                               where m.periodEnd >= dayFromPrevPeriod
                               select m;
                foreach (var item in searchTs)
                {
                    previousTimesheet = item;
                }
                //Iterate through each entry from previous week and duplicate it for this week
                var searchHours = from m in HoursDB.HoursList
                                  where (m.creator.CompareTo(userName) == 0)
                                  where m.timestamp >= previousTimesheet.periodStart
                                  where m.timestamp <= previousTimesheet.periodEnd
                                  select m;
                foreach (var item in searchHours)
                {
                    resultHours.Add(item);
                }
                foreach (var copiedHours in resultHours)
                {
                    copiedHours.hours = 0;
                    copiedHours.timestamp = DateTime.Now;
                    addHours(copiedHours);
                }
                return RedirectToAction("viewTimesheet", new { tsDate = DateTime.Now });
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
                //The View will only provide a link to previous timesheet if it exists
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
                    //save changes to the database
                    TimesheetDB.Entry(ts).State = System.Data.EntityState.Modified;
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
        public virtual List<SelectListItem> getDivisionSelectList()
        {
            Authentication auth = new Authentication();
            if (auth.isUser(this) || Authentication.DEBUG_bypassAuth)
            {
                List<SelectListItem> divList = new List<SelectListItem>();
                var searchDivisions = from m in DivisionsDB.DivisionsList
                                      select m;
                foreach (var item in searchDivisions)
                {
                    divList.Add(new SelectListItem
                    {
                        Text = item.divName,
                        Value = item.divName
                    }); 
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
                    Text = item.earningsCode.ToString(),
                    Value = item.earningsCode.ToString()
                });                    
            }
            return earnCodesList;
        }


        // 
        //Returns Earnings Code Descriptions for specified code as a list of strings
        public virtual List<string> getWorkTypeList(string earnCode)
        {
            List<string> workTypesList = new List<string>();
            var searchEarnCodes = from m in EarningsCodesDB.EarningsCodesList
                                  where (m.earningsCode.CompareTo(earnCode) == 0)
                                  select m;
            foreach (var item in searchEarnCodes)
            {
                workTypesList.Add(item.earningsCode + " " + item.description);
            }
            return workTypesList;
        }


        // 
        //Returns descriptions of all earnings codes associated with specified work effort
        public virtual List<string> getWorkEffortWorkTypeList(WorkEffort we)
        {
            List<string> workTypesList = new List<string>();
            var searchWorkTypes = from m in WorkTypeDB.WorkTypeList
                                  where m.WE == we.ID
                                  select m;
            foreach (var item in searchWorkTypes)
            {
                workTypesList.Add(item.description);
            }
            return workTypesList;
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
                var searchWorkEfforts = from m in WorkEffortDB.WorkEffortList
                                        where m.ID == id
                                        select m;
                string weDescription = searchWorkEfforts.First().description;
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


        //
        //
        public ActionResult jsonWorkEffortSelectList(string division)
        {
            List<SelectListItem> weSelectList = getVisibleWorkEffortSelectList(division);
            List<string> weIDList = new List<string>();
            foreach (var item in weSelectList)
            {
                weIDList.Add(item.Value);
            }
            return Json(weIDList.Select(x => new { value = x, text = getWeDescription(Convert.ToInt32(x)) }), 
                        JsonRequestBehavior.AllowGet
                        );
        }


        //
        //
        public ActionResult jsonWorkTypeSelectList(int weID)
        {
            WorkEffort we = WorkEffortDB.WorkEffortList.Find(weID);
            IEnumerable<string> weSelectList = getWorkEffortWorkTypeList(we);

            return Json(weSelectList.Select(x => new { value = x, text = x }),
                        JsonRequestBehavior.AllowGet
                        );
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
    }
}
