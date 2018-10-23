using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    [deOROWeb.Security.deORORoleAuthorize(Roles = "Admin")]
    public class ReconciliationController : MyBaseController
    {
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cash()
        {
            CashReconciliationController controller = new CashReconciliationController();
            return View(controller.GetAll());
        }

        public ActionResult Items()
        {
            CustomerRepository customer = new CustomerRepository();
            ViewBag.Customers = customer.GetAll();

            return View();
        }

        public JsonResult loadLocationServicedDates(int customerid, int locationid)
        {
            LocationServiceRepository repo = new LocationServiceRepository(customerid, locationid);
            var users = from c in repo.GetAll().Where(x=>x.comments == "Service Completed" && x.locationid == locationid).OrderByDescending(x=>x.created_date_time)
                        select new
                        {
                            id = c.id,
                            name = c.created_date_time
                        };

            return Json(users, JsonRequestBehavior.AllowGet);
        }

    }
}
