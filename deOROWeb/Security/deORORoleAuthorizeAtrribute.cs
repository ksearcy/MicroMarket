using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace deOROWeb.Security
{
    public class deORORoleAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        private string redirectUrl = "";
        public deORORoleAuthorizeAttribute()
            : base()
        {
        }

        public deORORoleAuthorizeAttribute(string redirectUrl)
            : base()
        {
            this.redirectUrl = redirectUrl;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.HttpContext.Response.Redirect(GetSiteRoot() + "/PageMissing/");
            }

            //else do normal process
            base.HandleUnauthorizedRequest(filterContext);
        }

        public static string GetSiteRoot()
        {
            string port = System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
            if (port == null || port == "80" || port == "443")
                port = "";
            else
                port = ":" + port;

            string protocol = System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"];
            if (protocol == null || protocol == "0")
                protocol = "http://";
            else
                protocol = "https://";

            string sOut = protocol + System.Web.HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + port + System.Web.HttpContext.Current.Request.ApplicationPath;

            if (sOut.EndsWith("/"))
            {
                sOut = sOut.Substring(0, sOut.Length - 1);
            }

            return sOut;
        }
    }
}