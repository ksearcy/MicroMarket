using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    public class PaymentController : MyBaseController
    {
        PaymentRepository repo = new PaymentRepository();

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult GetPayments(string pkid)
        {
            var payments = repo.GetAll().Where(x => x.shoppingcartpkid == pkid);
            return PartialView("Index",payments);
        }
    }
}
