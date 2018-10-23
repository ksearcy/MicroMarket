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
    public class ScheduleDetailController : MyBaseController
    {
        ScheduleDetailRepository repo1 = new ScheduleDetailRepository();
        ScheduleDetailItemRepository repo2 = new ScheduleDetailItemRepository();
        PlanogramRepository repo3 = new PlanogramRepository();
        UserRepository userRepo = new UserRepository();
        CategoryRepository repo4 = new CategoryRepository();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EditScheduleDetailItem(int id, int qtyRefill,string tote, string status)
        {
            var detailItem = repo2.GetSingleById(x => x.id == id);

            if (detailItem != null)
            {
                detailItem.quantity_to_refill = qtyRefill;
                detailItem.tote = tote;
                detailItem.status = status;
                repo2.Edit(detailItem);
                repo2.Save();
            }

            return View();
        }


        public int EditScheduleDetailItems(int detailid, string tote = null, string status = null)
        {
            var details = repo2.FindBy(x => x.scheduledetailid == detailid);
            int scheduleId = repo1.GetSingleById(x => x.id == detailid).scheduleid.Value;

            if (details != null)
            {
                foreach (var detail in details)
                {
                    if (tote != null)
                    {
                        detail.tote = tote == null ? detail.tote : tote;
                        detail.status = "1";
                    }
                    else
                    {
                        detail.status = status == "False" ? "0" : "1";
                    }

                    repo2.Edit(detail,x=>x.id == detail.id);
                }

                repo2.Save();
            }

            return scheduleId;
        }

        public int DeleteScheduleDetailItem(int id)
        {
            var detailItem = repo2.GetSingleById(x => x.id == id);
            int scheduleId = 0;

            if (detailItem != null)
            {
                int scheduleDetailId = detailItem.scheduledetailid.Value;
                scheduleId = repo1.GetSingleById(x => x.id == scheduleDetailId).scheduleid.Value;

                repo2.Delete(detailItem, x => x.id == id);
                repo2.Save();
            }

            //return RedirectToAction("EditPrePick", "Schedule", new { id = scheduleId });
            return scheduleId;
        }

        public ActionResult Details(int id = 0)
        {

            ViewBag.Users = userRepo.FindBy(x => x.is_staff == 1).ToList();
            ViewBag.Planograms = repo3.GetAll().ToList();
            ViewBag.Categories = repo4.GetSubCategories().ToList();

            List<Status> list = new List<Status>();
            list.Add(new Status { id = "Scheduled", name = "Scheduled" });
            list.Add(new Status { id = "Serviced", name = "Serviced" });
            list.Add(new Status { id = "Cancelled", name = "Cancelled" });

            ViewBag.Status = list;

            if (id == 0)
                return PartialView("Index", repo1.GetAll());
            else
            {
                return PartialView("Index", repo1.GetAll(id));
            }
        }

    }

    public class Status
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}