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

        public ActionResult TestViews()
        {
            var user = new UserController();
            ViewBag.userIndex              = user.Index()              != null;
            ViewBag.userAddHours           = user.addHours(DateTime.Now)           != null;
            ViewBag.userSearchWorkEffort   = user.searchWorkEffort()   != null;
            ViewBag.userViewWorkEffort     = user.viewWorkEffort(0)     != null;
            ViewBag.userViewTimesheet      = user.viewTimesheet(DateTime.Now)      != null;
            ViewBag.userViewHistory        = user.viewHistory()        != null;

            var manager = new ManagerController();
            ViewBag.managerIndex               = manager.Index()               != null;
            ViewBag.managerSearchPCA           = manager.searchPCA()           != null;
            ViewBag.managerAddWorkEffort       = manager.addWorkEffort()       != null;
            ViewBag.managerSearchWorkEffort    = manager.searchWorkEffort()    != null;
            ViewBag.managerViewWorkEffort      = manager.viewWorkEffort(0)     != null;
            ViewBag.managerEditWorkEffort      = manager.editWorkEffort(0)     != null;
            ViewBag.managerDeleteWorkEffort    = manager.deleteWorkEffort(0)   != null;

            var admin = new AdminController();
            ViewBag.adminAddUser            = admin.addUser()          != null;
            ViewBag.managerEditPCA = admin.editPCA(0) != null;
            ViewBag.managerDeletePCA = admin.deletePCA(0) != null;
            ViewBag.managerAddPCA = admin.addPCA() != null;

            return View();
        }
    }
}
