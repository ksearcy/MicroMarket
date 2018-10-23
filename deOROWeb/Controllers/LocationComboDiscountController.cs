using deORODataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace deOROWeb.Controllers
{
    public class LocationComboDiscountController : MyBaseController
    {
        LocationComboDiscountRepository repo = new LocationComboDiscountRepository();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddRemoveDiscounts(int locationid, int[] ids)
        {
            repo.AddRemoveDiscounts(locationid, ids);

            return View();
        }

        public JsonResult GetLocationComboDiscounts(int id)
        {
            var discounts = from c in repo.FindBy(x => x.locationid == id)
                        select new
                        {
                            id = c.combodiscountid,
                        };

            return Json(discounts, JsonRequestBehavior.AllowGet);
        }
    }
}
