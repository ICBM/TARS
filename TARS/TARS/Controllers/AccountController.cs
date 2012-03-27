using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Text;
using System.Collections;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

using TARS.Helpers;
using TARS.Models;

namespace TARS.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/LogOn
        public ActionResult LogOn()
        {
            
            //LDAPConnection ld = new LDAPConnection();
            //ld.establishConnection();
            return View();
        }

        //
        // POST: /Account/LogOn
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            LDAPConnection check = new LDAPConnection();

            if (ModelState.IsValid)
            {
//               if (check.requestUser(model.UserName, model.Password))
//                {
model.UserName = "zeke";
model.Password = "password";
model.RememberMe = false;
                    TARSUserDBContext TARSUserDB = new TARSUserDBContext();
//                    TARSUserDB.TARSUserList.Find(model.UserName);

                    //If the user is not an IDHW employee, then make sure his/her contractor information is up to date
                    if (model.costAllocated != true)
                    {
                        CheckContractorChanges(model);
                    }                

                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
//                }
/*                else
                {
                   ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
*/
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);
                
                if (createStatus == MembershipCreateStatus.Success)
                {
                    TARSUser newuser = new TARSUser();
                    newuser.userName = model.UserName;
                    newuser.permission = 1;
                    TARSUserDBContext user = new TARSUserDBContext();
                    user.TARSUserList.Add(newuser);
                    user.SaveChanges();

                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }


        //Function that checks if a contractor has changed employers 
        //If so, it saves the new employer name and start-date in TARSUsers table
        public void CheckContractorChanges(LogOnModel model)
        {
/*            int costAllocated = 0;

            PrincipalContext context = new PrincipalContext(ContextType.Domain, null, model.UserName, model.Password);
            UserPrincipal user = UserPrincipal.FindByIdentity(context, model.UserName);
            PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();
 
            foreach (GroupPrincipal group in groups)
            {
                costAllocated = String.Compare(group.SamAccountName, "Users");
                //If not part of "Users" group, then they aren't employed by IDHW
                if (costAllocated != 1)
                {
                    using (var context2 = new TARSUserDBContext())
                    {
                        //Query the TARSUser table for the current user
                        var userInDB = context2.TARSUserList
                                    .Where(u => u.userName == model.UserName)
                                    .FirstOrDefault();
                        //Update contractor info in table
                        userInDB.contractorName = group.Name;
                        userInDB.contractorStart = System.DateTime.Now;
                        context2.Entry(userInDB).State = EntityState.Modified;
                        context2.SaveChanges();
                    }
                    break;
                }
            }
*/
        }                                   
       
        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
