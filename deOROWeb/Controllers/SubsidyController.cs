using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    public class SubsidyController : MyBaseController
    {
        SubsidyRepository repo1 = new SubsidyRepository();
        SubsidyDetailRepository repo2 = new SubsidyDetailRepository();

        public ActionResult Index()
        {
            return View("Index", repo1.GetAll());
        }

        public JsonResult GetSubsidies()
        {
            var items = from c in repo1.GetAll().Where(x => x.is_active == 1)
                        select new
                        {
                            id = c.id,
                            name = c.description
                        };

            return Json(items, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Create([Bind(Exclude = "id")] subsidy subsidy, int[] ids)
        {
            if (ModelState.IsValid)
            {
                subsidy.created_date_time = DateTime.Now;
                repo1.Add(subsidy);
                repo1.Save();

                repo2.Add(subsidy.id, ids);
                repo2.Save();

                return RedirectToAction("Index");
            }

            return View("Index");
        }


        [HttpPost]
        public ActionResult Edit(subsidy subsidy, int[] ids)
        {
            if (ModelState.IsValid)
            {
                subsidy.modified_date_time = DateTime.Now;
                repo1.Edit(subsidy, ids);
                repo1.Save();
                return RedirectToAction("Index");
            }

            return View("Index");
        }

        public ActionResult Details(int id = 0)
        {
            var subsidy = repo1.GetSingleById(x => x.id == id);
            //var details = repo2.FindBy(x => x.subsidyid == id).Select(y => new { id = y.entityid });

            return Json(subsidy, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubsidyDetails(int id = 0)
        {
            if (id == 0)
                return Json(null, JsonRequestBehavior.AllowGet);

            return Json(repo2.FindBy(x => x.subsidyid == id).Select(y => new { id = y.entityid }), JsonRequestBehavior.AllowGet);
        }

        public dynamic GetSubsidiesHTML(int id = 0)
        {
            var subidies = "{";
            repo1.GetAll().Where(x => x.is_active == 1).ToList().ForEach(r =>
            {
                subidies += string.Format("\"{0}\":\"{1}\",", r.id, r.description);
            });
            subidies += "}";

            return Json(subidies, JsonRequestBehavior.AllowGet);
        }
    }
}
