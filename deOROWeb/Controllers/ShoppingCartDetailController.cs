using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    public class ShoppingCartDetailController : MyBaseController
    {
        ShoppingCartDetailRepository repo = new ShoppingCartDetailRepository();

        public ActionResult Index()
        {
            return View();
        }
        
        public PartialViewResult GetShoppingCartDetails(string pkid)
        {
            return PartialView("Index", repo.GetShoppingCartDetails(pkid));
        }
    }
}
