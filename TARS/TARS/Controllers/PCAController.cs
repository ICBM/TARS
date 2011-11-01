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
    public class PCAController : Controller
    {
        private PCA_CodeDBContext db = new PCA_CodeDBContext();

        //
        // GET: /PCA/

        public ViewResult Index()
        {
            return View(db.PCA_CodeList.ToList());
        }

        //
        // GET: /PCA/Details/5

        public ViewResult Details(int id)
        {
            PCA_Code pca_code = db.PCA_CodeList.Find(id);
            return View(pca_code);
        }

        //
        // GET: /PCA/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /PCA/Create

        [HttpPost]
        public ActionResult Create(PCA_Code pca_code)
        {
            if (ModelState.IsValid)
            {
                db.PCA_CodeList.Add(pca_code);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(pca_code);
        }
        
        //
        // GET: /PCA/Edit/5
 
        public ActionResult Edit(int id)
        {
            PCA_Code pca_code = db.PCA_CodeList.Find(id);
            return View(pca_code);
        }

        //
        // POST: /PCA/Edit/5

        [HttpPost]
        public ActionResult Edit(PCA_Code pca_code)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pca_code).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pca_code);
        }

        //
        // GET: /PCA/Delete/5
 
        public ActionResult Delete(int id)
        {
            PCA_Code pca_code = db.PCA_CodeList.Find(id);
            return View(pca_code);
        }

        //
        // POST: /PCA/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            PCA_Code pca_code = db.PCA_CodeList.Find(id);
            db.PCA_CodeList.Remove(pca_code);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}