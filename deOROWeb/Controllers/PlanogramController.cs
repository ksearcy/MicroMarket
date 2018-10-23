using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    [deOROWeb.Security.deORORoleAuthorize(Roles = "Admin")]
    public class PlanogramController : MyBaseController
    {
        PlanogramRepository repo1 = new PlanogramRepository();
        PlanogramItemRepository repo2 = new PlanogramItemRepository();

        public ActionResult Index(int id)
        {
            ViewBag.LocationId = id;
            return View();
        }

        public JsonResult GetPlanograms(int id = 0)
        {
            var items = from c in repo1.FindBy(x => x.locationid == id)
                        select new
                        {
                            id = c.id,
                            name = c.name
                        };

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPlanogramItem(int id)
        {
            var item = repo1.GetSingleById(x => x.id == id);
            return PartialView("PlanogramItem", item);
        }


        [HttpPost]
        public int Create([Bind(Exclude = "id")] planogram item)
        {
            if (ModelState.IsValid)
            {
                repo1.Add(item);
                repo1.Save();

                return item.id;
            }

            return 0;
        }

        [HttpPost]
        public ActionResult Edit(planogram item)
        {
            if (ModelState.IsValid)
            {
                string match1 = @"# 1. Literal text
		                        <!--
		                        # 2. Integer
		                        [0-9]+
		                        # 3. Literal text
		                        -->";
                string match2 = "[0-9]+";

                List<RootObject> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RootObject>>(item.data);
                List<int> itemids = new List<int>();
                
                foreach (RootObject r in list)
                {
                    r.htmlContent = "<li>" + r.htmlContent.Replace("\"", "'") + "</li>";

                    try
                    {
                        string commentMatch = Regex.Match(r.htmlContent, match1, RegexOptions.IgnorePatternWhitespace).Value;

                        if (commentMatch != "")
                        {
                            itemids.Add(Convert.ToInt32(Regex.Match(commentMatch, match2).Value));
                        }
                    }
                    catch { }

                }

                item.data = Newtonsoft.Json.JsonConvert.SerializeObject(list);

                repo1.Edit(item);
                repo1.Save();

                repo2.AddRemoveItems(item.id, itemids);

                return Index(item.locationid.Value);
            }

            return View("Index");
        }

        [HttpGet]
        public EmptyResult Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var item = repo1.GetSingleById(x => x.id == id);
                repo1.Delete(item, x => x.id == id);
                repo1.Save();
            }

            return new EmptyResult();
        }
    }


    public class RootObject
    {
        public int col { get; set; }
        public int row { get; set; }
        public int size_x { get; set; }
        public int size_y { get; set; }
        public string htmlContent { get; set; }
    }
}
