using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    [deOROWeb.Security.deORORoleAuthorize(Roles = "Admin")]
    public class ManufacturerController : MyBaseController
    {
        ManufacutrerRepository repo = new ManufacutrerRepository();

        public ActionResult Index()
        {
            return View("Index", repo.GetAll());
        }

        public ActionResult Details(int id = 0)
        {
            var manufacturer = repo.GetSingleById(x => x.id == id);

            if (manufacturer == null)
            {
                return HttpNotFound();
            }

            return Json(manufacturer, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(manufacturer manufacturer)
        {
            if (ModelState.IsValid)
            {
                repo.Edit(manufacturer);
                repo.Save();
                return RedirectToAction("Index");
            }

            return View("Index");
        }


        [HttpPost]
        public ActionResult Create([Bind(Exclude = "id")] manufacturer manufacturer)
        {
            if (ModelState.IsValid)
            {
                repo.Add(manufacturer);
                repo.Save();
                return RedirectToAction("Index");
            }

            return View("Index");
        }

        public JsonResult GetManufacturers()
        {
            var manufacturers = from c in repo.GetAll()
                                select new
                                {
                                    id = c.id,
                                    name = c.name
                                };

            return Json(manufacturers, JsonRequestBehavior.AllowGet);
        }
    }
}