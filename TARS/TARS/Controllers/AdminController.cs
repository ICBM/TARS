using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TARS.Helpers;

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

        public /*virtual*/ ActionResult addManager(string name)
        {
            Authentication auth = new Authentication();
            if (auth.isAdmin(this) || Authentication.DEBUG_bypassAuth)
            {
                ViewBag.name = name;
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
                return View(PcaDB.PcaCodeList.ToList());
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
    }
}
