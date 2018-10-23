using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace deOROWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //Your specialized route
            //routes.MapRoute(
            //    "LocationItem",
            //    "LocationItem/{locationid}",
            //    new { controller = "LocationItem", action = "IndexOverloaded", locationid = UrlParameter.Optional }
            //);

            //routes.MapRoute(
            //    "LocationItem",
            //    "LocationItem/{id}",
            //    new { controller = "LocationItem", action = "Index", locationid = UrlParameter.Optional }
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}