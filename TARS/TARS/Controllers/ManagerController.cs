﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TARS.Controllers
{
    public class ManagerController : UserController
    {
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
            ViewBag.code = code;
            return View();
        }

        public virtual ActionResult creatWorkEffort()
        {
            return null;
        }

        public virtual ActionResult viewWorkEffort()
        {
            return null;
        }

        public virtual ActionResult approveHours()
        {
            return null;
        }

        public virtual ActionResult editWorkEfforts()
        {
            return null;
        }

        public virtual ActionResult viewHistory()
        {
            return null;
        }

    }
}
