using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;
using System.Web.Services;

namespace deOROWeb.Controllers
{
    public class ShoppingCartController : MyBaseController
    {
        ShoppingCartRepository repo = new ShoppingCartRepository();

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult GetShoppingCarts(string userpkid, string fromDate, string toDate)
        {
            return PartialView("Index", repo.GetShoppingCarts(userpkid, fromDate, toDate));
        }

        public PartialViewResult GetShoppingCartFullDetail(string pkid)
        {
            return PartialView("ShoppingCartFullDetail", repo.GetShoppingCart(pkid));
        }


        //[WebMethod]
        //public JsonResult GetLiveShoppingCarts()
        //{
        //    string fromDate = DateTime.Now.AddMinutes(-5).ToString();
        //    string toDate = DateTime.Now.AddMinutes(-5).AddSeconds(11).ToString();

        //    var ShoppingCartTotals = repo.GetShoppingCartsAmountsTotals(fromDate, toDate);

        //    decimal ShoppingCartsTotal = 0;

        //    foreach (var element in ShoppingCartTotals)
        //    {
        //        ShoppingCartsTotal += element.price_tax_included;
        //    }

        //    List<string> ShoppingCartsTotalList = new List<string>();

        //    ShoppingCartsTotalList.Add(ShoppingCartsTotal.ToString());

        //    return Json(ShoppingCartsTotalList, JsonRequestBehavior.AllowGet);
        //}

    }
}
