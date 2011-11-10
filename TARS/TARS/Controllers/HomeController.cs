using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TARS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Update = "This is another ViewBag item";
            ViewBag.Message = "Welcome to TARS MVC Home controller";
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
