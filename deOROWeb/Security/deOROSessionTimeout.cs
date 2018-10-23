using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Security;
using System.Web.Mvc;

namespace deOROWeb.Security
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class deOROSessionTimeout : System.Web.Mvc.ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = filterContext.HttpContext;
            if (context.Session != null)
            {
                if (context.Session.IsNewSession)
                {
                    string sessionCookie = context.Request.Headers["Cookie"];
                    if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET&#95;SessionId") >= 0))
                    {
                        FormsAuthentication.SignOut();
                        string redirectTo = "~/Index/Login";
                        if (!string.IsNullOrEmpty(context.Request.RawUrl))
                        {
                            redirectTo = string.Format("~/Index/Login?ReturnUrl={0}", HttpUtility.UrlEncode(context.Request.RawUrl));
                        }
                        filterContext.HttpContext.Response.Redirect(redirectTo, true);
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}