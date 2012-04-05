using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TARS.Models;
using TARS.Helpers;

namespace TARS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to TARS";

            //LDAPConnection test = new LDAPConnection();

            //ViewBag.ldap = test.requestUser("Scott Beddall", "pass");

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
