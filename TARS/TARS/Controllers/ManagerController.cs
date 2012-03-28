using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using TARS.Helpers;
using TARS.Models;

namespace TARS.Controllers
{
    public class ManagerController : UserController
    {
        protected PCA_WEDBContext PCA_WEDB = new PCA_WEDBContext();
        
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
        // GET: /Manager/viewPCA/5
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


        public virtual ActionResult userManagement()
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                return View(TARSUserDB.TARSUserList.ToList());
            }
            else
            {
                return View("error");
            }
        }


        public virtual ActionResult weManagement()
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                return View(WorkEffortDB.WorkEffortList.ToList());
            }
            else
            {
                return View("error");
            }
        }

               
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
                string division = "";
                var nameSearch = from n in TARSUserDB.TARSUserList
                                 where (n.userName.CompareTo(this.User.Identity.Name) == 0)
                                 select n;
                foreach (var item in nameSearch)
                {
                    division = item.department;
                }
                ViewBag.pcaList = getPcaCodes(division);
                ViewBag.earnCodesList = getEarningsCodes();
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
                    WorkEffortDB.WorkEffortList.Add(workeffort);
                    WorkEffortDB.SaveChanges();
                    return RedirectToAction("weManagement");
                }
                return View(workeffort);
            }
            else
            {
                return View("error");
            }
        }

        //
        // GET: /Manager/searchWorkEffort
        //  - Shows a list of all WorkEffort codes.       
        public override ActionResult searchWorkEffort()
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                return View(WorkEffortDB.WorkEffortList.ToList());
            }
            else
            {
                return View("error");
            } 
        }
        
          
        //
        // GET: /Manager/viewWorkEffort/5
        //  - Shows detailed information for a single WorkEffort code.
        public override ActionResult viewWorkEffort(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                WorkEffort workeffort = WorkEffortDB.WorkEffortList.Find(id);
                return View(workeffort);
            }
            else
            {
                return View("error");
            } 
        }

        //
        // GET: /Manager/editWorkEffort/5
        //  - Edits a specific WorkEffort code.
        public virtual ActionResult editWorkEffort(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                WorkEffort workeffort = WorkEffortDB.WorkEffortList.Find(id);
                return View(workeffort);
            }
            else
            {
                return View("error");
            } 
        }

        //
        // POST: /Manager/editWorkEffort/5
        [HttpPost]
        public virtual ActionResult editWorkEffort(WorkEffort workeffort)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                if (ModelState.IsValid)
                {
                    WorkEffortDB.Entry(workeffort).State = EntityState.Modified;
                    WorkEffortDB.SaveChanges();
                    return RedirectToAction("weManagement/");
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
                return View(workeffort);
            }
            else
            {
                return View("error");
            } 
        }

        //
        // POST: /Manager/deleteWorkEffort/5
        [HttpPost, ActionName("deleteWorkEffort")] //This action MUST match the above delete function.
        public virtual ActionResult confirmedDeleteWorkEffort(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                WorkEffort workeffort = WorkEffortDB.WorkEffortList.Find(id);
                WorkEffortDB.WorkEffortList.Remove(workeffort);
                WorkEffortDB.SaveChanges();
                return RedirectToAction("weManagement/");
            }
            else
            {
                return View("error");
            } 
        }

        public virtual ActionResult hideWorkEffort(int id)
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
        // GET: /Manager/addPCA_WE
        public virtual ActionResult addPCA_WE()
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
        // POST: /Manager/addPCA_WE
        [HttpPost]
        public virtual ActionResult addPCA_WE(PCA_WE pca_we)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                if (ModelState.IsValid)
                {
                    PCA_WEDB.PCA_WEList.Add(pca_we);
                    PCA_WEDB.SaveChanges();
                    return RedirectToAction("searchPCA_WE/");
                }
                return View(pca_we);
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
        // GET: /Manager/viewPCA_WE/5
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
        // GET: /Manager/editPCA_WE/5
        //  - Edits a specific PCA_WE code.
        public virtual ActionResult editPCA_WE(int id)
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
        // POST: /Manager/editPCA_WE/5
        [HttpPost]
        public virtual ActionResult editPCA_WE(PCA_WE pca_we)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                if (ModelState.IsValid)
                {
                    PCA_WEDB.Entry(pca_we).State = EntityState.Modified;
                    PCA_WEDB.SaveChanges();
                    return RedirectToAction("searchPCA_WE/");
                }
                return View(pca_we);
            }
            else
            {
                return View("error");
            } 
        }

        //
        // GET: /Manager/deletePCA_WE/5
        public virtual ActionResult deletePCA_WE(int id)
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
        // POST: /Manager/deletePCA_WE/5
        [HttpPost, ActionName("deletePCA_WE")] //This action MUST match the above delete function.
        public virtual ActionResult confirmedDeletePCA_WE(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                PCA_WE pca_we = PCA_WEDB.PCA_WEList.Find(id);
                PCA_WEDB.PCA_WEList.Remove(pca_we);
                PCA_WEDB.SaveChanges();
                return RedirectToAction("searchPCA_WE/");
            }
            else
            {
                return View("error");
            } 
        }


        //
        //returns specified user's hours, along with manager options, to the View
        public virtual ActionResult approveTimesheet(int id)
        {
            DateTime tsDate = DateTime.Now;
            Authentication auth = new Authentication();
            if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
            {
                string user = "";

                var searchUsers = from n in TARSUserDB.TARSUserList
                                  where n.ID == id
                                  select n;
                var searchHours = from m in HoursDB.HoursList
                                  select m;
                foreach (var item in searchUsers)
                {
                    user = item.userName;
                }
                searchHours = from s in searchHours
                              where (s.creator.CompareTo(user) == 0)
                              select s;
                foreach (var item in searchHours)
                {
                    tsDate = item.timestamp;    //used to retrieve the correct timesheet
                }
                ViewBag.timesheet = getTimesheet(user, tsDate);
                return View(searchHours);
            }
            else
            {
                return View("error");
            }
        }

        //
        //changes timesheet status to approved
        public virtual ActionResult submitApproveTimesheet(int id)
        {
            if (id >= 0)
            {
                Authentication auth = new Authentication();
                if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
                {
                    Timesheet ts = new Timesheet();
                    ts = getTimesheetFromID(id);
                    ts.approved = true;
                    TimesheetDB.Entry(ts).State = System.Data.EntityState.Modified;
                    //save changes to the database
                    TimesheetDB.SaveChanges();

                    return RedirectToAction("approveTimesheet", new { id = getUserID(ts.worker) });
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
        //changes timesheet status to disapproved
        public virtual ActionResult submitDisapproveTimesheet(int id)
        {
            if (id >= 0)
            {
                Authentication auth = new Authentication();
                if (auth.isManager(this) || Authentication.DEBUG_bypassAuth)
                {
                    Timesheet ts = new Timesheet();
                    ts = getTimesheetFromID(id);
                    ts.approved = false;
                    ts.submitted = false;
                    TimesheetDB.Entry(ts).State = System.Data.EntityState.Modified;
                    //save changes to the database
                    TimesheetDB.SaveChanges();

                    return RedirectToAction("approveTimesheet", new { id = getUserID(ts.worker) });
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
        // GET: /Manager/managerSubmitTimesheet
        //changes timesheet status to true so it will show up in the manager's list of timesheets to approve
        public virtual ActionResult managerSubmitTimesheet(int id)
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

                    return RedirectToAction("approveTimesheet", new { id = getUserID(ts.worker) });
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
        //Returns the unique ID of specified user
        public int getUserID(string worker)
        {
            int userID = 0;
            var searchID = from m in TARSUserDB.TARSUserList
                           where (m.userName.CompareTo(worker) == 0)
                           select m;
            foreach (var item in searchID)
            {
                userID = item.ID;
            }
            return userID;
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
    }
}
