using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    public class WidgetController : MyBaseController
    {
        WidgetRepository repo = new WidgetRepository();

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult GetWidgets(int id)
        {
            var widgets = repo.FindBy(x => x.dashboardid == id);
            return PartialView("Index", widgets);
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "id")] widget widget)
        {
            if (ModelState.IsValid)
            {
                repo.Add(widget);
                repo.Save();
                return RedirectToAction("~/Dashboard/Index");
            }

            return View(widget);
        }

        [HttpPost]
        public ActionResult Edit(widget widget)
        {
            if (ModelState.IsValid)
            {
                repo.Edit(widget);
                repo.Save();
                return RedirectToAction("Index");
            }

            return View("Index");
        }

        [HttpPost]
        public void Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var widget = repo.GetSingleById(x => x.id == id);

                if (widget != null)
                {
                    repo.Delete(widget, x => x.id == id);
                    repo.Save();
                }
            }

        }


        [HttpPost]
        public void SaveSettings(int id, string title, int metricid, int order, int customerid, int locationid, string fromdate, string todate)
        {
            if (ModelState.IsValid)
            {
                widget widget = repo.GetSingleById(x => x.id == id);

                if (widget != null)
                {
                    widget.title = title;
                    widget.metricid = metricid;
                    widget.order = order;
                    widget.locationid = locationid;
                    widget.customerid = customerid;
                    try { widget.from_date = DateTime.Parse(fromdate); }
                    catch { widget.from_date = null; }
                    try { widget.to_date = DateTime.Parse(todate); }
                    catch { widget.to_date = null; }
                    repo.Edit(widget);
                    repo.Save();
                }
            }
        }

        public dynamic GetWidgetSettings(int id = 0)
        {
            if (id == 0)
                return new JavaScriptSerializer().Serialize(null);

            var widget = repo.GetSingleById(x => x.id == id);

            if (widget != null)
            {
                var w = new
                {
                    metricid = widget.metricid,
                    title = widget.title,
                    customerid = widget.customerid,
                    locationid = widget.locationid,
                    fromdate = widget.from_date,
                    todate = widget.to_date
                };

                return new JavaScriptSerializer().Serialize(w);
            }

            return new JavaScriptSerializer().Serialize(null);
        }

        public dynamic GetGridData(int metricid, int customerid, int locationid, string fromdate, string todate)
        {
            DataTable dt = new MetricController().GetMetricData(metricid, customerid, locationid, fromdate, todate);

            var columns = from d in dt.Columns.Cast<DataColumn>()
                          select new
                          {
                              sTitle = d.ColumnName,
                              sClass = "center"
                          };

            List<string[]> rows = new List<string[]>();


            foreach (DataRow row in dt.Rows)
            {
                string[] aRow = new string[columns.Count()];

                int i = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    aRow[i] = row[col].ToString();
                    i++;
                }
                rows.Add(aRow);
            }

            return new JavaScriptSerializer().Serialize(new { columns, rows });

        }

        public dynamic GetPieChartData(int metricid, int customerid, int locationid, string fromdate, string todate)
        {
            DataTable dt = new MetricController().GetMetricData(metricid, customerid, locationid, fromdate, todate);

            var items = from b in dt.AsEnumerable()
                        select new
                        {
                            label = b.Field<string>(1),
                            data = b.Field<Int32>(2)
                        };

            return new JavaScriptSerializer().Serialize(items);
        }

        public dynamic GetBarChartData(int metricid, int customerid, int locationid, string fromdate, string todate)
        {
            DataTable dt = new MetricController().GetMetricData(metricid, customerid, locationid, fromdate, todate);

            
            var items = from b in dt.AsEnumerable()
                        select new object[] 
                        {
                            b.Field<string>(1),
                            b.Field<Int32>(2)
                        };

            return new JavaScriptSerializer().Serialize(new { data = items, color = "#3c8dbc" });
        }
    }
}