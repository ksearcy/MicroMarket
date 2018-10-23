using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using deORODataAccess;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace deOROWeb.Controllers
{
    public class ScheduleController : MyBaseController
    {
        ScheduleRepository repo = new ScheduleRepository();
        ScheduleDetailRepository repoDetail = new ScheduleDetailRepository();
        ScheduleDetailItemRepository repoDetailItem = new ScheduleDetailItemRepository();
        UserRepository repoUser = new UserRepository();

        string prevColor = "";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id = 0)
        {
            schedule schedule = repo.GetSingleById(x => x.id == id);

            if (schedule == null)
            {
                return HttpNotFound();
            }

            return Json(schedule, JsonRequestBehavior.AllowGet);
        }


        public int ApplyFilter(int locationid, string categories, string planograms)
        {
            if (categories.Length == 1 && categories == "0")
                categories = null;

            if (planograms.Length == 1 && planograms == "0")
                planograms = null;

            return repoDetail.ApplyFilter(locationid, categories, planograms);
        }


        [HttpPost]
        public ActionResult Create(schedule schedule, List<deORODataAccess.DTO.ScheduleDetailDTO> tableData)
        {
                     

            var groupList = tableData.Where(x => x.selected == true && x.driverid != "0").GroupBy(x => x.driverid).Select(g => g.ToList());

            foreach (var g in groupList)
            {
                if (g.Count > 0)
                {
                    schedule.name += g[0].username + "(" + g.Count + "), ";
                }
            }
            if (schedule.name != null)
            {
                schedule.name = schedule.name.Substring(0, schedule.name.Length - 2);
                schedule.description = schedule.name;
            }
            repo.Add(schedule);
            repo.Save();

            foreach (var sd in tableData)
            {
                if (sd.selected)
                {
                    sd.excluded_categories = sd.excluded_categories.Replace(";", ",") ?? null;
                    sd.excluded_planograms = sd.excluded_planograms.Replace(";", ",") ?? null;

                    schedule_detail detail = new schedule_detail();
                    detail.customerid = sd.customerid;
                    detail.driverid = sd.driverid;
                    detail.locationid = sd.locationid;
                    detail.scheduleid = schedule.id;
                    detail.status = "Scheduled";
                    detail.status_updated_date_time = DateTime.Now;
                    detail.count = sd.count;
                    detail.excluded_categories = sd.excluded_categories;
                    detail.excluded_planograms = sd.excluded_planograms;

                    repoDetail.Add(detail);
                }
            }

            repoDetail.Save();

            foreach (var data in tableData)
            {
                repoDetailItem.AddItems(schedule.id, data.locationid, data.excluded_categories, data.excluded_planograms);
            }

            repoDetailItem.Save();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(schedule schedule, List<deORODataAccess.DTO.ScheduleDetailDTO> tableData)
        {
                       
            var groupList = tableData.Where(x => x.selected == true && x.driverid != "0").GroupBy(x => x.driverid).Select(g => g.ToList());

            foreach (var g in groupList)
            {
                if (g.Count > 0)
                {
                    schedule.name += g[0].username + "(" + g.Count + "), ";

                }
            }

            if (schedule.name != null)
            {
                schedule.name = schedule.name.Substring(0, schedule.name.Length - 2);
                schedule.description = schedule.name;
            }

            repo.Edit(schedule);
            repo.Save();

            repoDetail.AddRemoveItems(tableData);
            
            foreach (var data in tableData)
            {
                data.excluded_categories = data.excluded_categories.Replace(";", ",") ?? null;
                data.excluded_planograms = data.excluded_planograms.Replace(";", ",") ?? null;

                repoDetailItem.Delete(x => x.scheduledetailid == data.id);
                repoDetailItem.AddItems(schedule.id, data.locationid, data.excluded_categories, data.excluded_planograms);
            }

            repoDetailItem.Save();

            return View();
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var schedule = repo.GetSingleById(x => x.id == id);

                try
                {
                    if (schedule != null)
                    {
                        repo.Delete(schedule, x => x.id == id);

                        var scheduleDetail = repoDetail.GetSingleById(x => x.scheduleid == id);

                        if (scheduleDetail != null)
                        {
                            repoDetail.Delete(scheduleDetail, x => x.scheduleid == id);

                            var scheduleDetailItems = repoDetailItem.FindBy(x => x.scheduledetailid == scheduleDetail.id).ToList();

                            if (scheduleDetailItems.Count > 0)
                            {
                                scheduleDetailItems.ForEach(x =>
                                    {
                                        repoDetailItem.Delete(x, y => y.scheduledetailid == scheduleDetail.id);
                                    });
                            }

                            repoDetailItem.Save();
                        }

                        repoDetail.Save();
                    }

                    repo.Save();
                }
                catch { }

            }

            return RedirectToAction("Index");
        }


        public ViewResult PrintPreview(int id)
        {
            ViewBag.Schedule = repo.GetSingleById(x => x.id == id);
            return View("PrintPreview", repoDetail.GetAllForPrint(id));
        }

        public ViewResult EditPrePick(int id)
        {
            ViewBag.Schedule = repo.GetSingleById(x => x.id == id);
            return View("EditPrePick", repoDetail.GetAllForPrint(id));
        }

        public PartialViewResult PrintPreviewDetails(int id)
        {
            return PartialView("PrintPreviewScheduleDetails", repoDetailItem.GetAllForPrint(id));
        }

        public PartialViewResult PrePickList(int id)
        {
            return PartialView("PrePickList", repoDetailItem.GetAllForPrint(id));
        }

        public dynamic GetSchedules()
        {
            var schedules = (from c in repo.GetAll()
                             select c).ToList();

            List<CalendarEvent> events = new List<CalendarEvent>();

            foreach (var s in schedules)
            {
                events.Add(new CalendarEvent
                {
                    id = s.id.ToString(),
                    title = s.description,
                    start = s.date.Value.ToShortDateString(),
                    backgroundColor = GetRandomColor(),
                    borderColor = "#FFFFFF",
                    @class = "event-important",
                    allDay = true
                });
            }

            return new JavaScriptSerializer().Serialize(events);
        }

        public string GetRandomColor()
        {
            var random = new Random(DateTime.Now.Millisecond);
            var color = String.Format("#{0:X6}", new Random((int)DateTime.Now.Ticks).Next(0x1000000));

            if (prevColor == "")
            {
                prevColor = color;
                return prevColor;
            }

            while (prevColor == color)
            {
                color = String.Format("#{0:X6}", new Random((int)DateTime.Now.Ticks).Next(0x1000000));
            }

            prevColor = color;
            return prevColor;
        }
    }

    public class CalendarEvent
    {
        public string id;
        public string title;
        public string start;
        public string backgroundColor;
        public string borderColor;
        public string @class;
        public bool allDay;
    }
}