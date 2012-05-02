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
    public class AdminController : ManagerController
    {
        //
        // GET: /Admin/
        public override ActionResult Index()
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                return View();
            }
            else
            {
                return View("error");
            }
        }


        public ActionResult maintainPCA(string division = null)
        {
            Authentication auth = new Authentication();
            if(auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                if (TempData["failedPcaDelete"] != null)
                {
                    ViewBag.failedPcaDelete = true;
                }

                ViewBag.divisionList = getDivisionSelectList();
                if ((division == null) || (division.CompareTo("All") == 0))
                {
                    ViewBag.division = "All";
                    return View(PcaCodeDB.PcaCodeList.ToList());
                }
                else
                {
                    ViewBag.division = division;
                    var pcaList = from p in PcaCodeDB.PcaCodeList
                                  where (p.division.CompareTo(division) == 0)
                                  select p;
                    return View(pcaList.ToList());
                }   
            }
            else
            {
                return View("error");
            }
        }


        //
        // GET: /Admin/addPCA
        public virtual ActionResult addPCA()
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                ViewBag.divisionList = getDivisionSelectList();
                return View(new PcaCode());
            }
            else
            {
                return View("error");
            }
        }


        //
        // POST: /Admin/addPCA
        [HttpPost]
        public virtual ActionResult addPCA(PcaCode pcacode)
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                if (ModelState.IsValid)
                {
                    if (pcacode.endDate == null)
                    {
                        pcacode.endDate = DateTime.MaxValue;
                    }
                    //make sure the start date is before the end date
                    if (pcacode.startDate > pcacode.endDate)
                    {
                        ViewBag.endBeforeStartFlag = true;
                        ViewBag.divisionList = getDivisionSelectList();
                        return View(pcacode);
                    }

                    /* Make sure that the dates don't overlap if there is another PCA with the same
                     * code in the same division
                     */
                    if (pcaCheckIfDuplicate(pcacode) == false)
                    {
                        PcaCodeDB.PcaCodeList.Add(pcacode);
                        PcaCodeDB.SaveChanges();
                        return RedirectToAction("maintainPCA");
                    }
                    else
                    {
                        ViewBag.duplicatePcaFlag = true;
                        ViewBag.divisionList = getDivisionSelectList();
                        return View(pcacode);
                    }
                }
                return View(pcacode);
            }
            else
            {
                return View("error");
            }
        }


        // 
        // GET: /Admin/editPCA/
        //  - Edits a specific PCA code.
        public virtual ActionResult editPCA(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                PcaCode pcacode = PcaCodeDB.PcaCodeList.Find(id);
                ViewBag.divisionList = getDivisionSelectList();
                return View(pcacode);
            }
            else
            {
                return View("error");
            }
        }


        //
        // POST: /Admin/editPCA/
        [HttpPost]
        public virtual ActionResult editPCA(PcaCode pcacode)
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                //make sure startDate is prior to endDate 
                if (pcacode.startDate > pcacode.endDate)
                {
                    ViewBag.endBeforeStartFlag = true;
                    ViewBag.divisionList = getDivisionSelectList();
                    return View(pcacode);
                }
                
                /* Make sure that the dates don't overlap if there is another PCA with the same
                 * code in the same division
                 */
                if (pcaCheckIfDuplicate(pcacode) == true)
                {
                    ViewBag.duplicatePcaFlag = true;
                    ViewBag.divisionList = getDivisionSelectList();
                    return View(pcacode);
                }

                TempData["tmpPcaCode"] = pcacode;
                return RedirectToAction("confirmEditPCA");
            }
            else
            {
                return View("error");
            }
        }


        // 
        // GET: /Admin/confirmEditPCA/
        //  - gets confirmation before editing a specific PCA code.
        public virtual ActionResult confirmEditPCA()
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                ViewBag.divisionList = getDivisionSelectList();
                return View(TempData["tmpPcaCode"]);
            }
            else
            {
                return View("error");
            }
        }


        //
        // POST: /Admin/confirmEditPCA/
        //gets confirmation before editing a specific PCA code
        [HttpPost]
        public virtual ActionResult confirmEditPCA(PcaCode pcacode)
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {

                if (ModelState.IsValid)
                {
                    PcaCodeDB.PcaCodeList.Add(pcacode);
                    PcaCodeDB.Entry(pcacode).State = System.Data.EntityState.Modified;
                    PcaCodeDB.SaveChanges();
                }
                return RedirectToAction("maintainPCA");
            }
            else
            {
                return View("error");
            }
        }


        //
        // GET: /Admin/deletePCA
        public virtual ActionResult deletePCA(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
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
        // POST: /Admin/confirmDeletePCA
        [HttpPost, ActionName("deletePCA")] //This action MUST match the above delete function.
        public virtual ActionResult confirmDeletePCA(int id)
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                //make sure there are no work efforts attached to the PCA
                if (checkPcaForAttachedWorkEfforts(id) == true)
                {
                    TempData["failedPcaDelete"] = true;
                    return RedirectToAction("maintainPCA");
                }
                else
                {
                    PcaCode pcacode = PcaCodeDB.PcaCodeList.Find(id);
                    PcaCodeDB.PcaCodeList.Remove(pcacode);
                    PcaCodeDB.SaveChanges();
                    return RedirectToAction("maintainPCA");
                }
            }
            else
            {
                return View("error");
            }
        }


        //
        //Checks a PCA to see if any Work Efforts are currently attached to it. Returns TRUE if there are.
        public bool checkPcaForAttachedWorkEfforts(int id)
        {
            var searchPcaWe = from p in PCA_WEDB.PCA_WEList
                              where p.PCA == id
                              select p;
            foreach (var item in searchPcaWe)
            {
                //return true if there is an entry in PCA_WE table with a matching PCA id
                if (item != null)
                {
                    return true;
                }
                return false;
            }
            return false;
        }


        public ActionResult userMaintanence()
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                return View(TARSUserDB.TARSUserList.ToList());
            }
            else
            {
                return View("error");
            }
        }


        public ActionResult addUser()
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                return View();
            }
            else
            {
                return View("error");
            }
        }


        public ActionResult editTARSUSer()
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                return View();
            }
            else
            {
                return View("error");
            }
        }


        public ActionResult endDateTARSUSer()
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                return View();
            }
            else
            {
                return View("error");
            }
        }


        //
        /* Returns true if the PCA code already exists for the division and has 
         * overlapping start and end dates with the new pca code
         */
        public bool pcaCheckIfDuplicate(PcaCode pca)
        {
            bool existsFlag = false;
            var searchPca = from p in PcaCodeDB.PcaCodeList
                            where p.ID != pca.ID
                            where p.code == pca.code
                            where (p.division.CompareTo(pca.division) == 0) 
                            select p;
            foreach (var item in searchPca)
            {
                //if the date ranges overlap, then set flag to TRUE
                if ( (pca.startDate <= item.endDate)&&(item.startDate <= pca.endDate) )
                {
                    existsFlag = true;
                }
            }
            return existsFlag;
        }


        //
        // GET: /Admin/addPCA_WE
        //  Adds a PCA code to the given Work Effort
        public virtual ActionResult addPCA_WE(int weID)
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                WorkEffort we = WorkEffortDB.WorkEffortList.Find(weID);
                PcaCode pca = getPcaObjFromCode(we.pcaCode);
                ViewBag.workEffortDescription = we.description;
                ViewBag.workEffortId = weID;
                ViewBag.divisionList = getDivisionSelectList();
                ViewBag.pcaAddList = getDivisionPcaCodeList(pca.division);
                return View();
            }
            else
            {
                return View("error");
            }
        }


        //
        // POST: /Admin/addPCA_WE
        //  Adds a PCA code to the given Work Effort
        [HttpPost]
        public virtual ActionResult addPCA_WE(PCA_WE pca_we)
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                //The view actually passed the PCA code instead of the ID, so set the ID to the correct value
                pca_we.PCA = getPcaIdFromCode(pca_we.PCA);

                WorkEffort effort = WorkEffortDB.WorkEffortList.Find(pca_we.WE);
                ViewBag.workEffortDescription = effort.description;
                PcaCode pcaObj = PcaCodeDB.PcaCodeList.Find(pca_we.PCA);

                //make sure it falls within it's associated PCA code's time boundaries
                if (verifyWeTimeBounds(effort, pcaObj.code) == true)
                {
                    //Make sure it's not a duplicate entry before adding to database
                    if (checkIfDuplicatePcaWe(pca_we) == false)
                    {
                        //update PCA_WE table
                        PCA_WEDB.PCA_WEList.Add(pca_we);
                        PCA_WEDB.Entry(pca_we).State = System.Data.EntityState.Added;
                        PCA_WEDB.SaveChanges();
                    }
                    return RedirectToAction("editWorkEffort", "Manager", new { id = pca_we.WE });
                }
                ViewBag.divisionList = getDivisionSelectList();
                ViewBag.pcaAddList = getAllPcaCodes();
                ViewBag.workEffortId = effort.ID;
                ViewBag.outOfPcaTimeBounds = true;
                return View(pca_we);
            }
            else
            {
                return View("error");
            }
        }


        //
        // GET: /Admin/deletePCA_WE
        //  Deactivates a PCA_WE entry 
        public virtual ActionResult deletePCA_WE(int weID)
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                WorkEffort we = WorkEffortDB.WorkEffortList.Find(weID);
                ViewBag.workEffortDescription = we.description;
                ViewBag.workEffortId = weID;
                ViewBag.pcaList = getWePcaCodesList(we);
                return View();
            }
            else
            {
                return View("error");
            }
        }


        //
        // POST: /Admin/deletePCA_WE
        //  Deactivates the specified PCA_WE entry and changes the endDate to the current day
        [HttpPost]
        public virtual ActionResult deletePCA_WE(PCA_WE pca_we)
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                //The view actually passed the PCA code instead of the ID, so set the ID to the correct value
                pca_we.PCA = getPcaIdFromCode(pca_we.PCA);
                int count = 0;

                PCA_WE tmpPcaWe = new PCA_WE();
                var searchPcaWe = from p in PCA_WEDB.PCA_WEList
                                  where p.WE == pca_we.WE
                                  where p.active != true
                                  select p;
                foreach (var item in searchPcaWe)
                {
                    if (item.PCA == pca_we.PCA)
                    {
                        tmpPcaWe = PCA_WEDB.PCA_WEList.Find(item.ID);
                    }
                    count++;
                }

                //if it's not the last PCA_WE
                if (count > 1)
                {
                    /* save changes in database (pca_we.active was set to FALSE, and 
                     * pca_we.associationEndDate was set to DateTime.Now in the View)
                     */ 
                    PCA_WEDB.Entry(pca_we).State = System.Data.EntityState.Modified;
                    PCA_WEDB.SaveChanges();
                    return RedirectToAction("weManagement", "Manager");
                }

                ViewBag.lastPcaFlag = true;
                WorkEffort we = WorkEffortDB.WorkEffortList.Find(pca_we.WE);
                ViewBag.workEffortDescription = we.description;
                ViewBag.workEffortId = we.ID;
                ViewBag.pcaList = getWePcaCodesList(we);
                return View();
            }
            else
            {
                return View("error");
            }
        }


        //
        //Returns TRUE if the PCA_WE entry already exists
        public bool checkIfDuplicatePcaWe(PCA_WE pcawe)
        {
            var searchPcaWe = from p in PCA_WEDB.PCA_WEList
                              where p.WE == pcawe.WE
                              where p.PCA == pcawe.PCA
                              select p;
            foreach (var item in searchPcaWe)
            {
                if (item != null)
                {
                    return true;
                }
            }
            return false;
        }


        // 
        //Returns list of all the PCA codes in TARS
        public virtual List<string> getAllPcaCodes()
        {
            List<string> pcaList = new List<string>();
            string tmpPca = "";
            var searchPca = from m in PcaCodeDB.PcaCodeList
                            select m;
            foreach (var item in searchPca)
            {
                tmpPca = item.code.ToString();
                pcaList.Add(tmpPca);
            }
            return pcaList;
        }
    }
}
