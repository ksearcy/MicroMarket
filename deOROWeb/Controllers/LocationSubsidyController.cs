using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    public class LocationSubsidyController : MyBaseController
    {
        LocationSubsidyRepository repo = new LocationSubsidyRepository();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddRemoveSubsidies(int locationid, int[] ids)
        {
            repo.AddRemoveSubsidies(locationid, ids);
            return View();
        }
        
        public JsonResult GetLocationSubsidies(int id)
        {
            var subsidies = from c in repo.FindBy(x => x.locationid == id)
                            select new
                            {
                                id = c.subsidyid,
                            };

            return Json(subsidies, JsonRequestBehavior.AllowGet);
        }
    }
}
