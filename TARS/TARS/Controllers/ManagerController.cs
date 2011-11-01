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
        protected PCA_CodeDBContext db = new PCA_CodeDBContext();
        //
        // GET: /Manager/
        public override ActionResult Index() //Overridden from User/Index, which was virtual.
        {
            return View();
        }

        //
        // GET: /Manager/addPCA
        public virtual ActionResult addPCA(int code)
        {
            //fetch data from Form
            //send data to model
            //call appropriate view
            return null;
        }

        public virtual ActionResult viewPCA(int code)
        {
            PCA_Code pca = db.PCA_CodeList.Find(code);
            ViewBag.model = pca;
            ViewBag.code = code;
            //request data from model
            return View();
        }

        public virtual ActionResult editPCA()
        {
            //fetch data from Form
            //send data to model
            //call appropriate view
            return null;
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
