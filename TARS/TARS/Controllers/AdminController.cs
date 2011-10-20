using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TARS.Controllers
{
    public class AdminController : ManagerController
    {
        //
        // GET: /Admin/
        public override ActionResult Index()
        {
            return View();
        }

        public /*virtual*/ ActionResult addManager(string name)
        {
            ViewBag.name = name;
            return View();
        }
    }
}
