﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TARS.Models;

namespace TARS.Helpers
{
    public class Authentication
    {
        protected TARSUserDBContext TARSUserDB = new TARSUserDBContext();

        static public bool DEBUG_bypassAuth = false; //Bypass all permission checking if true

        protected int permission(Controller c)
        {
            var authed = c.Request.IsAuthenticated; //this completely breaks when used in test controller. the controller being called doesn't have a Request instance.
            if (authed == true)
            {
                var username = c.User.Identity.Name; //Grab username from the cookie.
                using (var context = TARSUserDB)
                {
                    var userInDB = context.TARSUserList
                                    .Where(u => u.userName == username)
                                    .FirstOrDefault();
                    if (userInDB == null)
                    {
                        TARSUser newuser = new TARSUser();
                        newuser.userName = username;
                        newuser.permission = 1;
                        TARSUserDB.TARSUserList.Add(newuser);
                        TARSUserDB.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        return userInDB.permission;
                    }
                }
            }
            return 0;
        }

        public bool isUser(Controller c)
        {
            bool b = permission(c) > 0; //Need a 1 or higher to access.
            return b;
        }
        public bool isManager(Controller c)
        {
            bool b = permission(c) > 1; //Need a 2 or higher to access.
            return b;
        }
        public bool isAdmin(Controller c)
        {
            bool b = permission(c) > 2; //Need a 3 or higher to access.
            return b;
        }
    }
}