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
    public class CategoryController : MyBaseController
    {
        CategoryRepository repo = new CategoryRepository();

        public ActionResult Index()
        {
            return View(repo.GetAll());
        }

        public ActionResult Details(int id = 0)
        {
            var category = repo.GetSingleById(id);

            if (category == null)
            {
                return HttpNotFound();
            }

            return Json(category, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(category category)
        {
            if (ModelState.IsValid)
            {
                repo.Edit(category);
                repo.Save();
                return RedirectToAction("Index");
            }

            return View("Index");
        }

        public JsonResult GetCategories()
        {
            var categories = from c in repo.GetCategories()
                            select new
                            {
                                id = c.id,
                                name = c.name
                            };

            var subcategories = from c in repo.GetSubCategories()
                             select new
                             {
                                 id = c.id,
                                 name = c.name,
                                 parentid = c.parentid
                             };

            return Json(new { categories, subcategories }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubCategories(int id)
        {
            var categories = from c in repo.GetSubCategories(id)
                            select new
                            {
                                id = c.id,
                                name = c.name
                            };

            return Json(categories, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Create([Bind(Exclude = "id")] category category)
        {

            int id = category.id;

            if (ModelState.IsValid)
            {
                repo.Add(category);
                repo.Save();
            }

            return View(category);
        }
    }
}