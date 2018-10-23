using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using deOROWeb.Models;

namespace deOROWeb.Controllers
{
    public class LoginController : MyBaseController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult Authenticate()
        //{
        //    FormsAuthentication.SetAuthCookie("deORO", false);
        //    return RedirectToAction("Index", "Dashboard");
        //}

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel model, string returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                MembershipProvider membership = new deOROWeb.Security.deOROMembershipProvider();

                if (membership.ValidateUser(model.UserName, model.Password))
                {
                    MembershipUser user = membership.GetUser(model.UserName, true);
                    Session["UserRole"] = user;

                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            return RedirectToAction("Index", "Login", null);

        }
    }
}
