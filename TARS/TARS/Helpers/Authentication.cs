using System;
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

        public int permission( Controller c )
        {
            var authed = c.Request.IsAuthenticated;
            if (authed)
            {
                var username = c.User.Identity.Name;
                using (var context = new TARSUserDBContext())
                {
                    var userInDB = context.TARSUserList
                                    .Where(u => u.un == username)
                                    .FirstOrDefault();
                    if (userInDB == null)
                    {
                        TARSUser newuser = new TARSUser();
                        newuser.un = username;
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
    }
}