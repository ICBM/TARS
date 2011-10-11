using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TARS.Controllers
{
    public class TestController : Controller
    {//NOTE: To access the controller, strip off 'Controller' from the class name.

        //
        // GET: /Test/
        public ActionResult Index() //Right click here to add a View. View is now in Views/Test/___.cshtml
        {
            return View();
        }

        // 
        // GET: /Test/Welcome/ 
        public string Welcome()
        {
            return "This is the Welcome action method...";
        }

        //public string Welcome(int number){return "#"+number;} // - Does NOT like function overloading.

        // GET: /Test/Parameters?name=""&number=3
        public ActionResult Parameters(string name, int number = 5)
        {
            ViewBag.message = "Message: " + name;
            ViewBag.number = number;
            return View();
        }
    }
}
