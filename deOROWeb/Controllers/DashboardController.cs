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
    public class DashboardController : MyBaseController
    {
        DashboardRepository repo = new DashboardRepository();

        public ActionResult Index()
        {
            //return Location();
            //return View();
            //return View("Dashboard1");

            CustomerRepository customer = new CustomerRepository();
            ViewBag.Customers = customer.GetAll();

            return View("Location");
        }

        public ActionResult Location()
        {
            CustomerRepository customer = new CustomerRepository();
            ViewBag.Customers = customer.GetAll();

            return View("Location");
        }

        public PartialViewResult GetHeader()
        {
            var dashboards = repo.GetAll();
            return PartialView("~/Views/Dashboard/Header.cshtml", dashboards);
        }

        public PartialViewResult GetContent()
        {
            var dashboards = repo.GetAll();
            return PartialView("~/Views/Dashboard/Content.cshtml", dashboards);
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "id")] dashboard dashboard)
        {
            if (ModelState.IsValid)
            {
                repo.Add(dashboard);
                repo.Save();
                return RedirectToAction("Index");
            }

            return View(dashboard);
        }

        public PartialViewResult GetLocationDashboardData(int id,string fromDate,string toDate)
        {
            LocationRepository repo = new LocationRepository();
            return PartialView("LocationDashboard", repo.GetDashboardData(id, fromDate, toDate));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var dashboard = repo.GetSingleById(x => x.id == id);

                if (dashboard != null)
                {
                    repo.Delete(dashboard, x => x.id == id);
                    repo.Save();
                }
            }

            return RedirectToAction("Index");
        }

        public JsonResult GetDashboards()
        {
            var dashboards = from c in repo.GetAll()
                             select new
                             {
                                 id = c.id,
                                 name = c.title
                             };

            return Json(dashboards, JsonRequestBehavior.AllowGet);
        }


    }
}