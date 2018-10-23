using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    public class LocationCreditUserController : MyBaseController
    {
        LocationCreditUserRepository repo = new LocationCreditUserRepository();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetLocationCreditUsers(int id)
        {
            var users = from c in repo.FindBy(x => x.creditid == id)
                            select new
                            {
                                id = c.userpkid,
                            };

            return Json(users, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AddRemoveUsers(int creditid, string[] userids)
        {
            repo.AddRemoveUsers(creditid, userids);
            return View();
        }

    }
}
