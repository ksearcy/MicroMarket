using deORODataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace deOROWeb.Controllers
{
    [deOROWeb.Security.deORORoleAuthorize(Roles = "Admin")]
    public class ComboDiscountController : MyBaseController
    {
        ComboDiscountRepository repo = new ComboDiscountRepository();
        ComboDiscountDetailRepository repo1 = new ComboDiscountDetailRepository();

        public ActionResult Index()
        {
            return View("Index", repo.GetAll());
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "id")] combo_discount discount, int[] ids)
        {
            if (ModelState.IsValid)
            {
                discount.created_date_time = DateTime.Now;
                repo.Add(discount);
                repo.Save();

                repo1.Add(discount.id, ids);
                repo1.Save();

                return RedirectToAction("Index");
            }

            return View("Index");
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
        public ActionResult Edit(combo_discount discount, int[] ids)
        {
            if (ModelState.IsValid)
            {
                discount.modified_date_time = DateTime.Now;
                repo.Edit(discount, ids);
                repo.Save();
                return RedirectToAction("Index");
            }

            return View("Index");
        }

        public ActionResult GetComboDiscountDetails(int id = 0)
        {
            if (id == 0)
                return Json(null, JsonRequestBehavior.AllowGet);

            return Json(repo1.FindBy(x => x.combodiscountid == id).Select(y => new { id = y.entityid }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetComboDiscounts()
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
