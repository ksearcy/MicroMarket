using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    public class LocationCreditController : MyBaseController
    {
        LocationCreditRepository repo1 = new LocationCreditRepository();
        LocationRepository repo2 = new LocationRepository();

        public ActionResult Index(int id = 0)
        {
            ViewBag.LocationDTO = repo2.GetSingleById(id);
            return View(repo1.GetAll(id));
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "id")] location_credit credit)
        {
            if (ModelState.IsValid)
            {
                credit.created_by = (Session["UserRole"] as MembershipUser).UserName;
                credit.created_date_time = DateTime.Now;
                repo1.Add(credit);
                repo1.Save();
            }

            return new EmptyResult();
        }

        public ActionResult Details(int id = 0)
        {
            var credit = repo1.GetSingleById(id);

            if (credit == null)
            {
                return HttpNotFound();
            }

            return Json(credit, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(location_credit credit)
        {
            if (ModelState.IsValid)
            {
                repo1.Edit(credit);
                repo1.Save();
            }

            return new EmptyResult();
        }
    }
}
