using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    [deOROWeb.Security.deORORoleAuthorize(Roles = "Admin")]
    public class CashReconciliationController : MyBaseController
    {
        CashReconciliationRepository repo1 = new CashReconciliationRepository();
        CashCollectionRepository repo2 = new CashCollectionRepository();

        public ActionResult Details(string id)
        {
            var recon = repo1.GetSingleById(x => x.cashcollectionpkid == id);
            return Json(recon, JsonRequestBehavior.AllowGet);
        }

        public dynamic GetAll()
        {
            return repo2.GetAll();
        }

        public ActionResult Save(int customerid, int locationid, string collectionPkid,
                                                 decimal? coinTotal = null, int? c1Total = null,
                                                 int? c2Total = null, int? c5Total = null, int? c10Total = null, int? c20Total = null,
                                                 int? c50Total = null, int? c100Total = null, decimal? Total = null)
        {
            cash_reconciliation cashRecon = repo1.GetSingleById(x => x.cashcollectionpkid == collectionPkid);

            if (cashRecon == null)
            {
                cashRecon = new cash_reconciliation();
                repo1.Add(cashRecon);

                var cashCollection = repo2.GetSingleById(x => x.pkid == collectionPkid);
                cashRecon.customerid = cashCollection.customerid;
                cashRecon.locationid = cashCollection.locationid;
                cashRecon.cashcollectionpkid = collectionPkid;

            }
            else
            {
                repo1.Edit(cashRecon);
            }

            cashRecon.username = User.Identity.Name;
            cashRecon.created_date_time = DateTime.Now;
            cashRecon.coin_total = coinTotal;
            cashRecon.c1_total = c1Total;
            cashRecon.c2_total = c2Total;
            cashRecon.c5_total = c5Total;
            cashRecon.c10_total = c10Total;
            cashRecon.c20_total = c20Total;
            cashRecon.c50_total = c50Total;
            cashRecon.c100_total = c100Total;
            cashRecon.total = Total;

            repo1.Save();

            return new EmptyResult();
        }
    }
}
