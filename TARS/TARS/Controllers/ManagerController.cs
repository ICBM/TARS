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
        protected PcaCodeDBContext PcaDB = new PcaCodeDBContext();
        protected WorkEffortDBContext WorkEffortDB = new WorkEffortDBContext();

        //
        // GET: /Manager/
        public override ActionResult Index() //Overridden from User/Index, which was virtual.
        {
            return View();
        }

        //
        // GET: /Manager/addPCA
        public virtual ActionResult addPCA()
        {
            return View();
        }

        //
        // POST: /Manager/addPCA
        [HttpPost]
        public virtual ActionResult addPCA(PcaCode pcacode)
        {
            if (ModelState.IsValid)
            {
                PcaDB.PcaCodeList.Add(pcacode);
                PcaDB.SaveChanges();
                return RedirectToAction("searchPCA/");
            }
            return View(pcacode);
        }

        //
        // GET: /Manager/searchPCA
        //  - Shows a list of all PCA codes.
        public virtual ActionResult searchPCA( )
        {
            return View(PcaDB.PcaCodeList.ToList());
        }

        //
        // GET: /Manager/viewPCA/5
        //  - Shows detailed information for a single PCA code.
        public virtual ActionResult viewPCA( int id )
        {
            PcaCode pcacode = PcaDB.PcaCodeList.Find( id );
            return View(pcacode);
        }

        // 
        // GET: /Manager/editPCA/5
        //  - Edits a specific PCA code.
        public virtual ActionResult editPCA( int id )
        {
            PcaCode pcacode = PcaDB.PcaCodeList.Find(id);
            return View(pcacode);
        }

        //
        // POST: /Manager/editPCA/5
        [HttpPost]
        public virtual ActionResult editPCA(PcaCode pcacode)
        {
            if (ModelState.IsValid)
            {
                PcaDB.Entry(pcacode).State = EntityState.Modified;
                PcaDB.SaveChanges();
                return RedirectToAction("searchPCA/");
            }
            return View(pcacode);
        }

        //
        // GET: /Manager/deletePCA/5
        public virtual ActionResult deletePCA(int id)
        {
            PcaCode pcacode = PcaDB.PcaCodeList.Find(id);
            return View(pcacode);
        }

        //
        // POST: /Manager/deletePCA/5
        [HttpPost, ActionName("deletePCA")] //This action MUST match the above delete function.
        public virtual ActionResult confirmedDeletePCA(int id)
        {
            PcaCode pcacode = PcaDB.PcaCodeList.Find(id);
            PcaDB.PcaCodeList.Remove(pcacode);
            PcaDB.SaveChanges();
            return RedirectToAction("searchPCA/");
        }
                //This was attached to delete; not sure what this is yet, but it doesn't explode!
                protected override void Dispose(bool disposing)
                {
                    PcaDB.Dispose();
                    base.Dispose(disposing);
                }










        //
        // GET: /Manager/addWorkEffort
        public virtual ActionResult addWorkEffort()
        {
            return View();
        }

        //
        // POST: /Manager/addWorkEffort
        [HttpPost]
        public virtual ActionResult addWorkEffort(WorkEffort workeffort)
        {
            if (ModelState.IsValid)
            {
                WorkEffortDB.WorkEffortList.Add(workeffort);
                WorkEffortDB.SaveChanges();
                return RedirectToAction("searchWorkEffort/");
            }
            return View(workeffort);
        }

        //
        // GET: /Manager/searchWorkEffort
        //  - Shows a list of all WorkEffort codes.
        public virtual ActionResult searchWorkEffort()
        {
            return View(WorkEffortDB.WorkEffortList.ToList());
        }

        //
        // GET: /Manager/viewWorkEffort/5
        //  - Shows detailed information for a single WorkEffort code.
        public virtual ActionResult viewWorkEffort(int id)
        {
            WorkEffort workeffort = WorkEffortDB.WorkEffortList.Find(id);
            return View(workeffort);
        }

        //
        // GET: /Manager/editWorkEffort/5
        //  - Edits a specific WorkEffort code.
        public virtual ActionResult editWorkEffort(int id)
        {
            WorkEffort workeffort = WorkEffortDB.WorkEffortList.Find(id);
            return View(workeffort);
        }

        //
        // POST: /Manager/editWorkEffort/5
        [HttpPost]
        public virtual ActionResult editWorkEffort(WorkEffort workeffort)
        {
            if (ModelState.IsValid)
            {
                WorkEffortDB.Entry(workeffort).State = EntityState.Modified;
                WorkEffortDB.SaveChanges();
                return RedirectToAction("searchWorkEffort/");
            }
            return View(workeffort);
        }

        //
        // GET: /Manager/deleteWorkEffort/5
        public virtual ActionResult deleteWorkEffort(int id)
        {
            WorkEffort workeffort = WorkEffortDB.WorkEffortList.Find(id);
            return View(workeffort);
        }

        //
        // POST: /Manager/deleteWorkEffort/5
        [HttpPost, ActionName("deleteWorkEffort")] //This action MUST match the above delete function.
        public virtual ActionResult confirmedDeleteWorkEffort(int id)
        {
            WorkEffort workeffort = WorkEffortDB.WorkEffortList.Find(id);
            WorkEffortDB.WorkEffortList.Remove(workeffort);
            WorkEffortDB.SaveChanges();
            return RedirectToAction("searchWorkEffort/");
        }

        















        public virtual ActionResult approveHours()
        {
            //fetch data from Form
            //send data to model
            //call appropriate view
            return null;
        }

        public override ActionResult viewHistory()
        {
            //fetch data from Form
            //request data from model
            //send data to appropriate view
            return null;
        }

    }
}
