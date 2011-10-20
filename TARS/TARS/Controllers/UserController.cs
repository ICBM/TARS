using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TARS.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult addHours(int workeffort = 0, int hours = 0)
        {
            ViewBag.message = "Message: ";
            ViewBag.workeffort = workeffort;
            ViewBag.number = hours;
            return View();
        }

    }
}
