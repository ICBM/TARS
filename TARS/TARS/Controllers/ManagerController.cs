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
        protected TaskDBContext TaskDB = new TaskDBContext();
        protected PCA_WEDBContext PCA_WEDB = new PCA_WEDBContext();
        
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
        
        public override ActionResult searchWorkEffort()
        {
            return View(WorkEffortDB.WorkEffortList.ToList());
        }
        
          
        //
        // GET: /Manager/viewWorkEffort/5
        //  - Shows detailed information for a single WorkEffort code.
        public override ActionResult viewWorkEffort(int id)
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








        //*****************************************************************
        //
        // GET: /Manager/addTask
        public virtual ActionResult addTask()
        {
            return View();
        }

        //
        // POST: /Manager/addTask
        [HttpPost]
        public virtual ActionResult addTask(Task task)
        {
            if (ModelState.IsValid)
            {
                TaskDB.TaskList.Add(task);
                TaskDB.SaveChanges();
                return RedirectToAction("searchTask/");
            }
            return View(task);
        }

        //
        // GET: /Manager/searchTask
        //  - Shows a list of all Task codes.
        public virtual ActionResult searchTask()
        {
            return View(TaskDB.TaskList.ToList());
        }

        //
        // GET: /Manager/viewTask/5
        //  - Shows detailed information for a single Task code.
        public virtual ActionResult viewTask(int id)
        {
            Task task = TaskDB.TaskList.Find(id);
            return View(task);
        }

        //
        // GET: /Manager/editTask/5
        //  - Edits a specific Task code.
        public virtual ActionResult editTask(int id)
        {
            Task task = TaskDB.TaskList.Find(id);
            return View(task);
        }

        //
        // POST: /Manager/editTask/5
        [HttpPost]
        public virtual ActionResult editTask(Task task)
        {
            if (ModelState.IsValid)
            {
                TaskDB.Entry(task).State = EntityState.Modified;
                TaskDB.SaveChanges();
                return RedirectToAction("searchTask/");
            }
            return View(task);
        }

        //
        // GET: /Manager/deleteTask/5
        public virtual ActionResult deleteTask(int id)
        {
            Task task = TaskDB.TaskList.Find(id);
            return View(task);
        }

        //
        // POST: /Manager/deleteTask/5
        [HttpPost, ActionName("deleteTask")] //This action MUST match the above delete function.
        public virtual ActionResult confirmedDeleteTask(int id)
        {
            Task task = TaskDB.TaskList.Find(id);
            TaskDB.TaskList.Remove(task);
            TaskDB.SaveChanges();
            return RedirectToAction("searchTask/");
        }
        //******************************************************************




        //*****************************************************************
        //
        // GET: /Manager/addPCA_WE
        public virtual ActionResult addPCA_WE()
        {
            return View();
        }

        //
        // POST: /Manager/addPCA_WE
        [HttpPost]
        public virtual ActionResult addPCA_WE(PCA_WE pca_we)
        {
            if (ModelState.IsValid)
            {
                PCA_WEDB.PCA_WEList.Add(pca_we);
                PCA_WEDB.SaveChanges();
                return RedirectToAction("searchPCA_WE/");
            }
            return View(pca_we);
        }

        //
        // GET: /Manager/searchPCA_WE
        //  - Shows a list of all PCA_WE codes.
        public virtual ActionResult searchPCA_WE()
        {
            return View(PCA_WEDB.PCA_WEList.ToList());
        }

        //
        // GET: /Manager/viewPCA_WE/5
        //  - Shows detailed information for a single PCA_WE code.
        public virtual ActionResult viewPCA_WE(int id)
        {
            PCA_WE pca_we = PCA_WEDB.PCA_WEList.Find(id);
            return View(pca_we);
        }

        //
        // GET: /Manager/editPCA_WE/5
        //  - Edits a specific PCA_WE code.
        public virtual ActionResult editPCA_WE(int id)
        {
            PCA_WE pca_we = PCA_WEDB.PCA_WEList.Find(id);
            return View(pca_we);
        }

        //
        // POST: /Manager/editPCA_WE/5
        [HttpPost]
        public virtual ActionResult editPCA_WE(PCA_WE pca_we)
        {
            if (ModelState.IsValid)
            {
                PCA_WEDB.Entry(pca_we).State = EntityState.Modified;
                PCA_WEDB.SaveChanges();
                return RedirectToAction("searchPCA_WE/");
            }
            return View(pca_we);
        }

        //
        // GET: /Manager/deletePCA_WE/5
        public virtual ActionResult deletePCA_WE(int id)
        {
            PCA_WE pca_we = PCA_WEDB.PCA_WEList.Find(id);
            return View(pca_we);
        }

        //
        // POST: /Manager/deletePCA_WE/5
        [HttpPost, ActionName("deletePCA_WE")] //This action MUST match the above delete function.
        public virtual ActionResult confirmedDeletePCA_WE(int id)
        {
            PCA_WE pca_we = PCA_WEDB.PCA_WEList.Find(id);
            PCA_WEDB.PCA_WEList.Remove(pca_we);
            PCA_WEDB.SaveChanges();
            return RedirectToAction("searchPCA_WE/");
        }
        //******************************************************************



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
