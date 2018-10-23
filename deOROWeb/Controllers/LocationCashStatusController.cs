using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    //[deOROWeb.Security.deORORoleAuthorize(Roles = "Admin")]
    public class LocationCashStatusController :  MyBaseController
    {
        LocationRepository repo = new LocationRepository();

        public PartialViewResult Index(int locationid)
        {
            return PartialView(repo.GetLocationCashStatus(locationid));
        }

    }
}
