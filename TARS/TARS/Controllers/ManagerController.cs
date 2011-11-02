using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TARS.Models;

namespace TARS.Controllers
{
    public class ManagerController : UserController
    {
        protected PcaCodeDBContext db = new PcaCodeDBContext();
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
                db.PcaCodeList.Add(pcacode);
                db.SaveChanges();
                return RedirectToAction("searchPCA/");
            }
            return View(pcacode);
        }

        //
        // GET: /Manager/searchPCA
        //  - Shows a list of all PCA codes.
        public virtual ActionResult searchPCA( )
        {
            return View(db.PcaCodeList.ToList());
        }

        //
        // GET: /Manager/viewPCA/5
        //  - Shows detailed information for a single PCA code.
        public virtual ActionResult viewPCA( int id )
        {
            PcaCode pcacode = db.PcaCodeList.Find( id );
            return View(pcacode);
        }

        // 
        // GET: /Manager/editPCA/5
        //  - Edits a specific PCA code.
        public virtual ActionResult editPCA( int id )
        {
            PcaCode pcacode = db.PcaCodeList.Find(id);
            return View(pcacode);
        }

        //
        // POST: /Manager/editPCA/5
        [HttpPost]
        public virtual ActionResult editPCA(PcaCode pcacode)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pcacode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("searchPCA/");
            }
            return View(pcacode);
        }

        //
        // GET: /Manager/deletePCA/5
        public virtual ActionResult deletePCA(int id)
        {
            PcaCode pcacode = db.PcaCodeList.Find(id);
            return View(pcacode);
        }

        //
        // POST: /Manager/deletePCA/5
        [HttpPost, ActionName("deletePCA")] //This action MUST match the above delete function.
        public virtual ActionResult confirmedDeletePCA(int id)
        {
            PcaCode pcacode = db.PcaCodeList.Find(id);
            db.PcaCodeList.Remove(pcacode);
            db.SaveChanges();
            return RedirectToAction("searchPCA/");
        }
                //This was attached to delete; not sure what this is yet, but it doesn't explode!
                protected override void Dispose(bool disposing)
                {
                    db.Dispose();
                    base.Dispose(disposing);
                }









        public virtual ActionResult createWorkEffort()
        {
            //fetch data from Form
            //request data from model
            //send data to appropriate view
            return null;
            
        }

        public virtual ActionResult viewWorkEffort(string name)
        {
            ViewBag.name = name;
            //request data from model
            return View();
        }

        public virtual ActionResult editWorkEfforts()
        {
            //fetch data from Form
            //request data from model
            //call appropriate view
            return null;
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
