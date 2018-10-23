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
    public class WebUserController : MyBaseController
    {
        WebUserRepository repo = new WebUserRepository();

        public ActionResult Index()
        {
            return View("Index", repo.GetAll());
        }

        public ActionResult Details(int id = 0)
        {
            var webuser = repo.GetSingleById(x => x.id == id);

            if (webuser == null)
            {
                return HttpNotFound();
            }

            return Json(webuser, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(webuser webuser)
        {
            if (ModelState.IsValid)
            {
                repo.Edit(webuser);
                repo.Save();
                return RedirectToAction("Index");
            }

            return View("Index");
        }


        [HttpPost]
        public ActionResult Create([Bind(Exclude = "id")] webuser webuser)
        {
            if (ModelState.IsValid)
            {
                repo.Add(webuser);
                repo.Save();
                return RedirectToAction("Index");
            }

            return View("Index");
        }

        public PartialViewResult GetProfile(string userName)
        {
            var user = repo.GetSingleById(x => x.username == userName);
            return PartialView("~/Views/WebUser/Profile.cshtml", user);
        }


        [HttpPost]
        public JsonResult IsUsernameAvailable(string input_user_name, string input_hidden_action)
        {
            if (input_hidden_action != "")
            {
                return new JsonResult
                {
                    Data = true,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            var user = repo.GetSingleById(x => x.username == input_user_name);

            if (user != null)
            {
                return new JsonResult
                {
                    Data = false,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
                return new JsonResult
                {
                    Data = true,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

        }

        [HttpPost]
        public JsonResult IsEmailAvailable(string input_email, string input_hidden_action)
        {
            var user = repo.GetSingleById(x => x.email == input_email);

            if (user != null)
            {
                if (user.id == Convert.ToInt32(input_hidden_action))
                {
                    return new JsonResult
                    {
                        Data = true,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    return new JsonResult
                    {
                        Data = false,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            else
                return new JsonResult
                {
                    Data = true,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

        }
    }
}