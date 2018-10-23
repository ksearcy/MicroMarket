using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    [AllowAnonymous]
    public class ForgotPasswordController : MyBaseController
    {
        WebUserRepository repo = new WebUserRepository();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public bool SendPassword(string email)
        {
            var user = repo.GetSingleById(x => x.email == email);

            if (user != null)
            {
                try
                {
                    Helper.EmailHelper.Instance.SendPassword(user.email, user.username, user.password);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [HttpGet]
        public string ValidateEmail(string email)
        {
            var user = repo.GetSingleById(x => x.email == email);

            if (user != null)
                return email;
            else
                return null;
        }
    }
}
