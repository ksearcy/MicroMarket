using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    public class LocationCreditActivityController : MyBaseController
    {
        LocationCreditActivityRepository repo = new LocationCreditActivityRepository();

        public ActionResult Index(int id = 0)
        {
            return View("Index", repo.GetAll(id));
        }
    }
}
