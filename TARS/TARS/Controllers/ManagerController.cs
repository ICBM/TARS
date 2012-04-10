using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;

using TARS.Helpers;
using TARS.Models;

namespace TARS.Controllers
{
    public class ManagerController : UserController
    {        
        //
        // GET: /Manager/
        public override ActionResult Index() //Overridden from User/Index, which was virtual.
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                return View();
            }
            else
            {
                return View("error");
            }
        }


        //
        // GET: /Manager/searchPCA
        //  - Shows a list of all PCA codes.
        public virtual ActionResult searchPCA( )
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                return View(PcaCodeDB.PcaCodeList.ToList());
            }
            else
            {
                return View("error");
            } 
        }


        //
        // GET: /Manager/viewPCA
        //  - Shows detailed information for a single PCA code.
        public virtual ActionResult viewPCA( int id )
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                PcaCode pcacode = PcaCodeDB.PcaCodeList.Find(id);
                return View(pcacode);
            }
            else
            {
                return View("error");
            }
        }


        //
        //
        public virtual ActionResult userManagement()
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                //if it's a redirect from submitRejectTimesheet()
                if (TempData["emailSentFlag"] != null)
                {
                    ViewBag.emailSentFlag = true;
                    ViewBag.messageRecipient = TempData["recipient"];
                    ViewBag.emailError = TempData["emailError"];
                }
                return View(TARSUserDB.TARSUserList.ToList());
            }
            else
            {
                return View("error");
            }
        }


        //
        //
        public virtual ActionResult weManagement()
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
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

                //check if an "unable to hide Work Effort error should be displayed"
                if (TempData["failedHide"] != null)
                {
                    ViewBag.failedHide = true;
                }
                //check if an "unable to delete Work Effort error should be displayed"
                if (TempData["failedDelete"] != null)
                {
                    ViewBag.failedDelete = true;
                }
                return View(workEffortList);
            }
            else
            {
                return View("error");
            }
        }


        //       
        //This was attached to delete; not sure what this is yet, but it doesn't explode!
        protected override void Dispose(bool disposing)
        {
            PcaCodeDB.Dispose();
            base.Dispose(disposing);
        }


        //
        // GET: /Manager/addWorkEffort
        public virtual ActionResult addWorkEffort()
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                ViewBag.divisionList = getDivisionSelectList();
                ViewBag.earningsCodeSelectList = getEarningsCodeSelectList();
                return View();
            }
            else
            {
                return View("error");
            }
        }


        //
        // POST: /Manager/addWorkEffort
        [HttpPost]
        public virtual ActionResult addWorkEffort(WorkEffort workeffort)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                if (ModelState.IsValid)
                {
                    //make sure it falls within it's associated PCA code's time boundaries
                    if (verifyWeTimeBounds(workeffort, workeffort.pcaCode) == true)
                    {
                        //update WorkEffort table in database
                        WorkEffortDB.WorkEffortList.Add(workeffort);
                        WorkEffortDB.SaveChanges();

                        //add the WorkEffort/WorkTypes association to WorkTypes table
                        WorkType wType = new WorkType();
                        foreach (var item in workeffort.workTypes)
                        {
                            wType.WE = workeffort.ID;
                            wType.description = item;
                            WorkTypeDB.WorkTypeList.Add(wType);
                            WorkTypeDB.SaveChanges();
                        }

                        //add the PCA_WE association to PCA_WE table
                        PCA_WE tmpPcaWe = new PCA_WE();
                        tmpPcaWe.WE = workeffort.ID;
                        tmpPcaWe.PCA = getPcaIdFromCode(workeffort.pcaCode);
                        PCA_WEDB.PCA_WEList.Add(tmpPcaWe);
                        PCA_WEDB.SaveChanges();

                        return RedirectToAction("weManagement");
                    }
                    else
                    {
                        ViewBag.divisionList = getDivisionSelectList();
                        ViewBag.earningsCodeSelectList = getEarningsCodeSelectList();
                        ViewBag.notWithinTimeBounds = true;
                        return View(workeffort);
                    }
                }
                return View("error");
            }
            else
            {
                return View("error");
            }
        }


        //
        // GET: /Manager/editWorkEffort
        //  - Edits a specific WorkEffort code.
        public virtual ActionResult editWorkEffort(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                WorkEffort workeffort = WorkEffortDB.WorkEffortList.Find(id);
                ViewBag.pcaList = getWorkEffortPcaCodes(workeffort);
                ViewBag.workTypesList = getWorkEffortWorkTypeList(workeffort);
                string division = getUserDivision();
                ViewBag.divisionName = division;

                Authentication newAuth = new Authentication();
                if (newAuth.isAdmin(this))
                {
                    ViewBag.adminFlag = true;
                }
                return View(workeffort);
            }
            else
            {
                return View("error");
            } 
        }


        //
        // POST: /Manager/editWorkEffort
        [HttpPost]
        public virtual ActionResult editWorkEffort(WorkEffort workeffort)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                if (ModelState.IsValid)
                {
                    //make sure it falls within it's associated PCA code's time boundaries
                    if (verifyWeTimeBounds(workeffort, workeffort.pcaCode) == true)
                    {
                        WorkEffortDB.WorkEffortList.Add(workeffort);
                        WorkEffortDB.Entry(workeffort).State = System.Data.EntityState.Modified;
                        WorkEffortDB.SaveChanges();
                        return RedirectToAction("weManagement");
                    }
                    else
                    {
                        ViewBag.notWithinTimeBounds = true;
                        ViewBag.pcaList = getWorkEffortPcaCodes(workeffort);
                        ViewBag.workTypesList = getWorkEffortWorkTypeList(workeffort);

                        Authentication newAuth = new Authentication();
                        if (newAuth.isAdmin(this))
                        {
                            ViewBag.adminFlag = true;
                        }
                        return View(workeffort);
                    }
                }
                return View(workeffort);
            }
            else
            {
                return View("error");
            }
        }


        //
        // GET: /Manager/deleteWorkEffort/5
        public virtual ActionResult deleteWorkEffort(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                WorkEffort workeffort = WorkEffortDB.WorkEffortList.Find(id);
                ViewBag.pcaList = getWorkEffortPcaCodes(workeffort);
                return View(workeffort);
            }
            else
            {
                return View("error");
            } 
        }


        //
        // POST: /Manager/deleteWorkEffort
        [HttpPost, ActionName("deleteWorkEffort")] //This action MUST match the above delete function.
        public virtual ActionResult confirmedDeleteWorkEffort(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                //make sure that there aren't any hours billed to the work effort
                if (checkWeForBilledHours(id) == true)
                {
                    TempData["failedDelete"] = true;
                    return RedirectToAction("weManagement"); 
                }
                else
                {
                    WorkEffort workeffort = WorkEffortDB.WorkEffortList.Find(id);
                    //delete the PCA_WE entries for the work effort
                    deleteAllPcaWeForWorkEffort(id);
                    //delete the work effort
                    WorkEffortDB.WorkEffortList.Remove(workeffort);
                    WorkEffortDB.SaveChanges();
                    return RedirectToAction("weManagement");
                }
            }
            else
            {
                return View("error");
            } 
        }


        //
        // GET: /Manager/addWE_WeType
        //  Adds Work Type(s) to the given Work Effort
        public virtual ActionResult addWE_WeType(int weID)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                WorkEffort we = WorkEffortDB.WorkEffortList.Find(weID);
                ViewBag.workTypeAddList = getWorkTypeList(we.earningsCode);
                return View(we);
            }
            else
            {
                return View("error");
            }
        }


        //
        // POST: /Manager/addWE_WeType
        //  Adds Work Type(s) to the given Work Effort
        [HttpPost]
        public virtual ActionResult addWE_WeType(WorkEffort tmpWe)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                WorkType wType = new WorkType();
                foreach (var item in tmpWe.workTypes)
                {
                    wType.WE = tmpWe.ID;
                    wType.description = item;
                    //add each selected work type to the WorkType table
                    WorkTypeDB.WorkTypeList.Add(wType);
                    WorkTypeDB.SaveChanges();
                }
                return RedirectToAction("editWorkEffort", new { id = tmpWe.ID });
            }
            else
            {
                return View("error");
            }
        }


        //
        //Deletes all entries in PCA_WE table for specified Work Effort
        public void deleteAllPcaWeForWorkEffort(int id)
        {
            List<PCA_WE> tmpPcaWe = new List<PCA_WE>();
            var searchPcaWe = from p in PCA_WEDB.PCA_WEList
                              where p.WE == id
                              select p;
            tmpPcaWe.AddRange(searchPcaWe);
            foreach (var item in tmpPcaWe)
            {
                //delete the association from the PCA_WE table
                PCA_WEDB.PCA_WEList.Remove(item);
                PCA_WEDB.SaveChanges();
            }
            return;
        }


        //
        //Checks a work effort to see if any hours are currently billed to it. Returns TRUE if there are.
        public bool checkWeForBilledHours(int id)
        {
            Timesheet tmpTimesheet = new Timesheet();

            //get all hours that have been billed to the work effort
            var searchHours = from h in HoursDB.HoursList
                              where h.workEffortID == id
                              select h;
            foreach (var hrs in searchHours)
            {
                //If the corresponding timesheet is still active, return TRUE
                tmpTimesheet = getTimesheet(hrs.creator, hrs.timestamp);
                if (tmpTimesheet.locked == false)
                {
                    return true;
                }
            }
            return false;       //if there are no hours billed to it, return false
        }


        //
        //Marks an expired work effort as hidden so it doesn't show up for employees
        public virtual ActionResult hideWorkEffort(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                WorkEffort we = WorkEffortDB.WorkEffortList.Find(id);
                if (we.endDate < DateTime.Today)
                {
                    we.hidden = true;
                    WorkEffortDB.Entry(we).State = System.Data.EntityState.Modified;
                    WorkEffortDB.SaveChanges();
                    return RedirectToAction("WeManagement");
                }
                else
                {
                    TempData["failedHide"] = true;
                    return RedirectToAction("WeManagement");
                }
            }
            else
            {
                return View("error");
            }
        }


        //
        //Marks a hidden work effort as not hidden so it shows up for employees
        public virtual ActionResult unHideWorkEffort(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                WorkEffort we = WorkEffortDB.WorkEffortList.Find(id);
                we.hidden = false;
                WorkEffortDB.Entry(we).State = System.Data.EntityState.Modified;
                WorkEffortDB.SaveChanges();

                return RedirectToAction("WeManagement");
            }
            else
            {
                return View("error");
            }
        }


        //
        // GET: /Manager/searchPCA_WE
        //  - Shows a list of all PCA_WE codes.
        public virtual ActionResult searchPCA_WE()
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                return View(PCA_WEDB.PCA_WEList.ToList());
            }
            else
            {
                return View("error");
            } 
        }


        //
        // GET: /Manager/viewPCA_WE
        //  - Shows detailed information for a single PCA_WE code.
        public virtual ActionResult viewPCA_WE(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                PCA_WE pca_we = PCA_WEDB.PCA_WEList.Find(id);
                return View(pca_we);
            }
            else
            {
                return View("error");
            } 
        }


        //
        //GET: Manager/approveTimesheet
        //Gets hours for specified user for the time period that tsDate falls within
        public virtual ActionResult approveTimesheet(int userKeyID, DateTime tsDate)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                TARSUser employee = TARSUserDB.TARSUserList.Find(userKeyID);
                Timesheet timesheet = getTimesheet(employee.userName, tsDate);

                if (timesheet == null)
                {
                    createCurrentTimesheet(employee.userName);
                    ViewBag.timesheet = getTimesheet(employee.userName, DateTime.Now);
                }
                else
                {
                    ViewBag.timesheet = timesheet;
                }

                var searchHours = from m in HoursDB.HoursList
                                  where (m.creator.CompareTo(employee.userName) == 0)
                                  where m.timestamp >= timesheet.periodStart
                                  where m.timestamp <= timesheet.periodEnd
                                  select m;
                return View(searchHours);
            }
            else
            {
                return View("error");
            }
        }


        //
        //changes timesheet status to submitted (if not already) and approved
        public virtual ActionResult submitApproveTimesheet(int id)
        {
            if (id >= 0)
            {
                Authentication auth = new Authentication();
                if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
                {
                    Timesheet ts = new Timesheet();
                    ts = TimesheetDB.TimesheetList.Find(id);
                    ts.submitted = true;
                    ts.approved = true;
                    TimesheetDB.Entry(ts).State = System.Data.EntityState.Modified;
                    //save changes to the database
                    TimesheetDB.SaveChanges();

                    return RedirectToAction("userManagement");
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
        //changes timesheet submitted status to false and sends an alert email to employee
        public virtual ActionResult submitRejectTimesheet(int id)
        {
            if (id >= 0)
            {
                Authentication auth = new Authentication();
                if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
                {
                    Timesheet ts = new Timesheet();
                    ts = TimesheetDB.TimesheetList.Find(id);
                    ts.submitted = false;
                    ts.approved = false;
                    //save changes to the database
                    TimesheetDB.Entry(ts).State = System.Data.EntityState.Modified;
                    TimesheetDB.SaveChanges();

                    //send an email to employee to notify them
                    sendRejectedTimesheetEmail(ts.worker);
                    TempData["emailSentFlag"] = true;
                    TempData["recipient"] = ts.worker;
                    TempData["emailError"] = TempData["emailError"];

                    return RedirectToAction("userManagement");
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
        //Sends an email to inform employee that their timesheet was rejected
        public bool sendRejectedTimesheetEmail(string userName)
        {
//NEED TO LOOK IN ACTIVE DIRECTORY TO GET THIS DONE CORRECTLY
string toAddress = "zeke_long@hotmail.com";
//string toAddress = getEmailAddress(userName);

            string subject = "IDHW Rejected Timesheet";
            string body = "Hello, <br /><br />This is an email to inform you that your IDHW timesheet" +
                          " has been rejected by a manager.<br /> Please log in to TARS and fix" +
                          " any errors, then re-submit as soon as possible.<br /><br /> Thanks!";
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add(new MailAddress(toAddress));
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                var client = new SmtpClient();
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                TempData["emailError"] = ex;
            }
        
            return true;
        }


        //
        // GET: /Manager/managerSubmitTimesheet
        //changes the specified timesheet submitted status to true
        public virtual ActionResult managerSubmitTimesheet(int id)
        {
            if (id >= 0)
            {
                Authentication auth = new Authentication();
                if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
                {
                    Timesheet ts = new Timesheet();
                    ts = TimesheetDB.TimesheetList.Find(id);
                    ts.submitted = true;
                    TimesheetDB.Entry(ts).State = System.Data.EntityState.Modified;
                    //save changes to the database
                    TimesheetDB.SaveChanges();

                    return RedirectToAction("approveTimesheet", new { id = getUserKeyID(ts.worker) });
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


        // GET: /Manager/managerEditHours
        //  - Edits a specified Hours entry
        public virtual ActionResult managerEditHours(int hrsID, int tsID)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                Hours hours = HoursDB.HoursList.Find(hrsID);
                Timesheet timesheet = TimesheetDB.TimesheetList.Find(tsID);
                ViewBag.timesheetLockedFlag = timesheet.locked;
                Authentication newAuth = new Authentication();
                bool adminFlag = newAuth.isAdmin(this);
                ViewBag.adminFlag = adminFlag;
                ViewBag.userName = timesheet.worker;
                ViewBag.workEffort = WorkEffortDB.WorkEffortList.Find(hours.workEffortID);
                return View(hours);
            }
            else
            {
                return View("error");
            }
        }


        //
        // POST: /Manager/managerEditHours
        [HttpPost]
        public virtual ActionResult managerEditHours(Hours tmpHours)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                if (ModelState.IsValid)
                {
                    HoursDB.Entry(tmpHours).State = EntityState.Modified;
                    HoursDB.SaveChanges();
                }
                int userKeyID = getUserKeyID(tmpHours.creator);
                return RedirectToAction("approveTimesheet", new { userKeyID = userKeyID, tsDate = tmpHours.timestamp });
            }
            else
            {
                return View("error");
            }
        }


        //
        //Returns the unique ID of specified user
        public int getUserKeyID(string worker)
        {
            int userID = 0;
            var searchID = from m in TARSUserDB.TARSUserList
                           where (m.userName.CompareTo(worker) == 0)
                           select m;
            userID = searchID.First().ID;
            return userID;
        }


        //
        //Returns the unique ID of specified PCA code
        public int getPcaIdFromCode(int pcacode)
        {
            int pcaID = 0;
            var searchID = from m in PcaCodeDB.PcaCodeList
                           where m.code == pcacode
                           select m;
            pcaID = searchID.First().ID;
            return pcaID;
        }


        //
        //Returns a PcaCode object with the specified PCA code
        public PcaCode getPcaFromCode(int pcacode)
        {
            PcaCode pcaCodeObj = new PcaCode();
            var searchPca = from m in PcaCodeDB.PcaCodeList
                           where m.code == pcacode
                           select m;
            pcaCodeObj = searchPca.First();
            return pcaCodeObj;
        }


        // 
        //Returns PCA Codes as a selection list
        public virtual List<string> getDivisionPcaCodeList(string division)
        {
            List<string> pcaCodesList = new List<string>();
            var searchPcaCodes = from m in PcaCodeDB.PcaCodeList
                                 where (m.division.CompareTo(division) == 0)
                                 select m;
            foreach (var item in searchPcaCodes)
            {
                pcaCodesList.Add(item.code.ToString());
            }
            return pcaCodesList;
        }


        //
        //Checks to see if the work effort falls within it's PCA code's time boundaries
        public bool verifyWeTimeBounds(WorkEffort effort, int pca)
        {
            bool dateFlag = false;
            var searchPCA = from m in PcaCodeDB.PcaCodeList
                           where (m.code.CompareTo(pca) == 0)
                           select m;
            foreach (var item in searchPCA)
            {
                if ((item.startDate <= effort.startDate) && (item.endDate >= effort.endDate))
                {
                    dateFlag = true;
                }
            }
            return dateFlag;
        }


        //
        //Returns PCA code's time boundaries as a string
        //(note: it's called from addWorkEffort View)
        public string getPcaTimeBoundsString(int pcacode)
        {
            PcaCode tmpPca = new PcaCode();
            string bounds = "";
            var searchPCA = from m in PcaCodeDB.PcaCodeList
                            where (m.code.CompareTo(pcacode) == 0)
                            select m;
            foreach (var item in searchPCA)
            {
                bounds = tmpPca.startDate.ToShortDateString() + " - " + tmpPca.endDate.ToShortDateString();
            }
            return bounds;
        }


        //
        //
        public override ActionResult viewHistory()
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                //fetch data from Form
                //request data from model
                //send data to appropriate view
                return null;
            }
            else
            {
                return View("error");
            }
        }


        //
        //
        public ActionResult jsonPcaSelectList(string division)
        {
            IEnumerable<string> pcaList = getDivisionPcaCodeList(division);

            return Json(pcaList.Select(x => new { value = x, text = x }),
                        JsonRequestBehavior.AllowGet
                        );
        }


        //
        //
        public ActionResult jsonWorkTypeSelectList(string earnCode)
        {
            IEnumerable<string> workTypeList = getWorkTypeList(earnCode);

            return Json(workTypeList.Select(x => new { value = x, text = x }),
                        JsonRequestBehavior.AllowGet
                        );
        }
    }
}
