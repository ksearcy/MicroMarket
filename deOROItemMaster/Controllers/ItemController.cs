using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace deOROImageMaster.Controllers
{
    public class ItemController : Controller
    {
        deORO_ItemsEntities entities = new deORO_ItemsEntities();

        public ActionResult Index()
        {
            var items = (from i in entities.items
                         select new deOROImageMaster.Models.ItemDTO
                         {
                             id = i.ID,
                             upc = i.UPC,
                             brand = i.Brand,
                             description = i.Item_Description,
                             manufacturer = i.Manufacturer
                         }).ToList().Take(1);

            return View(items);
        }

        public string GetMediumImageUrl(string upc)
        {
            if (System.IO.File.Exists(string.Format(Request.PhysicalApplicationPath + @"\Images\Medium\{0}-medium-image.png", upc)))
            {
                string path = string.Format("/Images/Medium/{0}-medium-image.png", upc.PadLeft(14, '0'));
                return GetSiteRoot() + path;
            }

            return "";

        }

        public string GetSmallImageUrl(string upc)
        {
            if (System.IO.File.Exists(string.Format(Request.PhysicalApplicationPath + @"\Images\Small\{0}-small-image.png", upc)))
            {
                string path = string.Format("/Images/Small/{0}-small-image.png", upc.PadLeft(14, '0'));
                return GetSiteRoot() + path;
            }

            return "";

        }

        public string GetLargeImageUrl(string upc)
        {
            if (System.IO.File.Exists(string.Format(Request.PhysicalApplicationPath + @"\Images\Large\{0}-large-image.png", upc)))
            {
                string path = string.Format("/Images/Large/{0}-large-image.png", upc.PadLeft(14, '0'));
                return GetSiteRoot() + path;
            }

            return "";

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
