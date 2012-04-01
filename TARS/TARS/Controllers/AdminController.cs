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


        public ActionResult maintainPCA()
        {
            Authentication auth = new Authentication();
            if(auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                return View(PcaCodeDB.PcaCodeList.ToList());
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
                ViewBag.divisionList = getDivisions();
                return View();
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
                    //make sure the pca code doesn't already exist in the same division
                    if (pcaCheckIfDuplicate(pcacode) == false)
                    {
                        PcaCodeDB.PcaCodeList.Add(pcacode);
                        PcaCodeDB.SaveChanges();
                        return RedirectToAction("maintainPCA/");
                    }
                    else
                    {
                        ViewBag.duplicatePcaFlag = true;
                        ViewBag.divisionList = getDivisions();
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
                PcaCode pcacode = new PcaCode();
                pcacode = PcaCodeDB.PcaCodeList.Find(id);
                ViewBag.divisionList = getDivisions();
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
                if (pcacode.startDate.CompareTo(pcacode.endDate) > 0)
                {
                    return View("error");
                }
                //make sure the pca code doesn't already exist in the same division
                if (pcaCheckIfDuplicate(pcacode) == true)
                {
                    ViewBag.duplicatePcaFlag = true;
                    ViewBag.divisionList = getDivisions();
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
                ViewBag.divisionList = getDivisions();
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
                PcaCode pcacode = PcaCodeDB.PcaCodeList.Find(id);
                PcaCodeDB.PcaCodeList.Remove(pcacode);
                PcaCodeDB.SaveChanges();
                return RedirectToAction("maintainPCA/");
            }
            else
            {
                return View("error");
            }
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
        //Returns true if the PCA code already exists for the division 
        public bool pcaCheckIfDuplicate(PcaCode pca)
        {
            bool existsFlag = false;
            var searchPca = from p in PcaCodeDB.PcaCodeList
                            where p.code == pca.code
                            select p;
            foreach (var item in searchPca)
            {
                if (item.code > 0)
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
                ViewBag.workEffortDescription = we.description;
                ViewBag.workEffortId = weID;
                ViewBag.pcaAddList = getAllPcaCodes();
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
                    //update PCA_WE table in database
                    PCA_WEDB.PCA_WEList.Add(pca_we);
                    PCA_WEDB.Entry(pca_we).State = System.Data.EntityState.Added;
                    PCA_WEDB.SaveChanges();
                    return RedirectToAction("editWorkEffort", "Manager", new { id = pca_we.WE });
                }
                ViewBag.pcaAddList = getAllPcaCodes();
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
        //  Deletes a PCA code from the given Work Effort
        public virtual ActionResult deletePCA_WE(int weID)
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                WorkEffort we = WorkEffortDB.WorkEffortList.Find(weID);
                ViewBag.workEffortDescription = we.description;
                ViewBag.workEffortId = weID;
                ViewBag.pcaList = getWorkEffortPcaCodes(we);
                return View();
            }
            else
            {
                return View("error");
            }
        }


        //
        // POST: /Admin/deletePCA_WE
        //  Deletes a PCA code to the given Work Effort
        [HttpPost]
        public virtual ActionResult deletePCA_WE(PCA_WE pca_we)
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                //The view actually passed the PCA code instead of the ID, so set the ID to the correct value
                pca_we.PCA = getPcaIdFromCode(pca_we.PCA);
                int count = 0;

                PCA_WE tmpPca = new PCA_WE();
                var searchPcaWe = from p in PCA_WEDB.PCA_WEList
                                  where p.PCA == pca_we.PCA
                                  where p.WE == pca_we.WE
                                  select p;
                foreach (var item in searchPcaWe)
                {
                    tmpPca = PCA_WEDB.PCA_WEList.Find(item.ID);
                    count += 1;
                }

                //if it's not the last PCA_WE, then delete 
                if (count > 1)
                {
                    PCA_WEDB.PCA_WEList.Remove(tmpPca);
                    PCA_WEDB.SaveChanges();
                    return RedirectToAction("editWorkEffort", "Manager", new { id = pca_we.WE });
                }

                ViewBag.lastPcaFlag = true;
                WorkEffort we = WorkEffortDB.WorkEffortList.Find(pca_we.WE);
                ViewBag.workEffortDescription = we.description;
                ViewBag.workEffortId = we.ID;
                ViewBag.pcaList = getWorkEffortPcaCodes(we);
                return View();
            }
            else
            {
                return View("error");
            }
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
