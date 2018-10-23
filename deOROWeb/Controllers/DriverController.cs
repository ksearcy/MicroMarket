using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    [deOROWeb.Security.deORORoleAuthorize(Roles = "Admin")]
    public class DriverController : MyBaseController
    {
        DriverRepsoitory repo = new DriverRepsoitory();

        public ActionResult Index()
        {
            return View("Index", repo.GetAll());
        }

        public ActionResult Details(int id = 0)
        {
            var driver = repo.GetSingleById(x => x.id == id);

            if (driver == null)
            {
                return HttpNotFound();
            }

            return Json(driver, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(driver driver)
        {
            if (ModelState.IsValid)
            {
                repo.Edit(driver);
                repo.Save();
                return RedirectToAction("Index");
            }

            return View("Index");
        }


        [HttpPost]
        public ActionResult Create([Bind(Exclude = "id")] driver driver)
        {
            if (ModelState.IsValid)
            {
                repo.Add(driver);
                repo.Save();
                return RedirectToAction("Index");
            }

            return View("Index");
        }

        public JsonResult GetDrivers()
        {
            var drivers = "{";
            repo.GetAll().ToList().ForEach(r =>
            {
                drivers += string.Format("\"{0}\":\"{1}\",", r.id, r.name);
            });
            drivers += "}";

            return Json(drivers, JsonRequestBehavior.AllowGet);

        }

    }
}
