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
    [deOROWeb.Security.deORORoleAuthorize(Roles = "Admin")]
    public class UserDeletedController : MyBaseController
    {
        UserDeletedRepository repo = new UserDeletedRepository();

        public ActionResult Index()
        {
            return View(repo.GetAll());
        }

        public string UpdateData(int? id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            users_deleted user = repo.GetSingleById(x => x.id == id);

            if (user != null)
            {
                if (columnName.Equals("Refund Processed"))
                {
                    user.refund_processed = Convert.ToByte(value);
                }
                if (columnName.Equals("Refund Cleared"))
                {
                    user.refund_cleared = Convert.ToByte(value);
                }

                repo.Edit(user);
                repo.Save();
                return value;
            }
            else
            {
                return "";
            }
        }
    }
}