using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    public class AccountBalanceHistoryController : MyBaseController
    {
        AccountBalanceHistoryRepository repo = new AccountBalanceHistoryRepository();

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult GetHistories(string userpkid, string fromDate, string toDate)
        {
            return PartialView("Index", repo.GetHistories(userpkid, fromDate, toDate));
        }
    }
}
