using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using deORODataAccess;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace deOROWeb.Controllers
{
    [deOROWeb.Security.deORORoleAuthorize(Roles = "Admin")]
    public class DiscountController : MyBaseController
    {
        DiscountRepository repo = new DiscountRepository();

        public ActionResult Index()
        {
            return View("Index", repo.GetAll());
        }

        public ActionResult Details(int id = 0)
        {
            var discount = repo.GetSingleById(x => x.id == id);

            if (discount == null)
            {
                return HttpNotFound();
            }

            return Json(discount, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(discount discount)
        {
            if (ModelState.IsValid)
            {
                discount.modified_date_time = DateTime.Now;
                repo.Edit(discount);
                repo.Save();
                return RedirectToAction("Index");
            }

            return View("Index");
        }


        [HttpPost]
        public ActionResult Create([Bind(Exclude = "id")] discount discount)
        {
            if (ModelState.IsValid)
            {
                discount.created_date_time = DateTime.Now;
                repo.Add(discount);
                repo.Save();
                return RedirectToAction("Index");
            }

            return View("Index");
        }

        public dynamic GetDiscountsHTML(int id = 0)
        {
            var discounts = "{";
            repo.GetAll().ToList().ForEach(r =>
            {
                discounts += string.Format("\"{0}\":\"{1}\",", r.id, r.description);
            });
            discounts += "}";

            return Json(discounts, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDiscounts()
        {
            var items = from c in repo.GetAll()
                        select new
                        {
                            id = c.id,
                            name = c.description
                        };

            return Json(items, JsonRequestBehavior.AllowGet);
        }
    }
}