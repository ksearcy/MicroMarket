using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;


namespace deOROWeb.Controllers
{
    public class MetricController : MyBaseController
    {
        MetricRepository repo = new MetricRepository();

        public JsonResult GetMetrics()
        {
            var metrics = from c in repo.GetAll().OrderBy(x => x.name)
                          select new
                          {
                              id = c.id,
                              name = c.name
                          };

            return Json(metrics, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetMetricsByWidgetType(string widgetType)
        {
            var metrics = from c in repo.GetAll().Where(x => x.widget_type.Contains(widgetType)).OrderBy(x => x.name)
                          select new
                          {
                              id = c.id,
                              name = c.name,
                          };

            return Json(metrics, JsonRequestBehavior.AllowGet);
        }

        public DataTable GetMetricData(int id, int customerid, int locationid, string fromdate, string todate)
        {
            DataTable dt = new DataTable();
            var metric = repo.GetSingleById(x => x.id == id);

            if (metric != null)
            {
                string query = "";
                string dateQuery = metric.date_range;

                if (customerid != -1)
                    query += " AND o.customerid = " + customerid;

                if (locationid != -1)
                    query += " AND o.locationid = " + locationid;

                if (fromdate != "")
                    dateQuery = dateQuery.Replace("{0}", fromdate);
                else
                    dateQuery = dateQuery.Replace("{0}", "1/1/1900");

                if (todate != "")
                    dateQuery = dateQuery.Replace("{1}", todate);
                else
                    dateQuery = dateQuery.Replace("{1}", "12/31/2099");

                dt = Helper.DbHelper.ExecuteDataTable(repo.GetConnectionString(), string.Format(metric.query, query, dateQuery));
            }

            return dt;
        }

        public DataTable GetMetricData(int id, int customerid, int[] locationids, string fromdate, string todate)
        {
            DataTable dt = new DataTable();
            var metric = repo.GetSingleById(x => x.id == id);

            if (metric != null)
            {
                string query = "";

                if (customerid != -1)
                    query += " AND o.customerid = " + customerid;

                if (locationids != null)
                {
                    string locationQuery = " AND o.locationid IN (";
                    for (int i = 0; i < locationids.Length; i++)
                    {
                        locationQuery += locationids[i] + ",";
                    }
                    locationQuery = locationQuery.Substring(0, locationQuery.Length - 1) + ") ";

                    query += locationQuery;
                }

                string dateQuery = metric.date_range;

                if (dateQuery != "" && dateQuery != null)
                {
                    if (fromdate != "")
                    {
                        if (!fromdate.Contains(":"))
                            dateQuery = dateQuery.Replace("{0}", fromdate + " 00:00:00 AM");
                        else
                            dateQuery = dateQuery.Replace("{0}", fromdate);
                    }
                    else
                        dateQuery = dateQuery.Replace("{0}", "1/1/1900 00:00:00 AM");

                    if (todate != "")
                    {
                        if (!todate.Contains(":")){
                            dateQuery = dateQuery.Replace("{1}", todate + "  11:59:59 PM");
                        }
                        else{
                            if (todate.Contains("12:00:00 AM"))
                            {
                                dateQuery = dateQuery.Replace("{1}", todate.Replace("12:00:00 AM","11:59:59 PM"));
                            }
                            else {
                                dateQuery = dateQuery.Replace("{1}", todate);
                            }
                        }
                    }
                    else{
                        dateQuery = dateQuery.Replace("{1}", "12/31/2099 11:59:59 PM");
                    }
                }


                dt = Helper.DbHelper.ExecuteDataTable(repo.GetConnectionString(), string.Format(metric.query, query, dateQuery));
            }

            return dt;
        }

        public DataTable GetMetricData(int id, int customerid, int[] locationids, string fromdate, string todate, string additionalCondition)
        {
            DataTable dt = new DataTable();
            var metric = repo.GetSingleById(x => x.id == id);

            if (metric != null)
            {
                string query = "";
                string dateQuery = metric.date_range;

                if (customerid != -1)
                    query += " AND o.customerid = " + customerid;

                if (locationids != null)
                {
                    string locationQuery = " AND o.locationid IN (";
                    for (int i = 0; i < locationids.Length; i++)
                    {
                        locationQuery += locationids[i] + ",";
                    }
                    locationQuery = locationQuery.Substring(0, locationQuery.Length - 1) + ") ";

                    query += locationQuery;
                }

                if (fromdate != "")
                {
                    if (!fromdate.Contains(":"))
                    {
                        dateQuery = dateQuery.Replace("{0}", fromdate + " 00:00:00 AM");
                    }
                    else
                    {
                        dateQuery = dateQuery.Replace("{0}", fromdate);
                    }

                }
                else
                {
                    dateQuery = dateQuery.Replace("{0}", "1/1/1900 00:00:00 AM");
                }
                if (todate != "")
                {
                    if (!todate.Contains(":"))
                    {
                        dateQuery = dateQuery.Replace("{1}", todate + "  11:59:59 PM");
                    }
                    else
                    {
                          if (todate.Contains("12:00:00 AM"))
                            {
                                dateQuery = dateQuery.Replace("{1}", todate.Replace("12:00:00 AM","11:59:59 PM"));
                            }
                            else {
                                dateQuery = dateQuery.Replace("{1}", todate);
                            }
                    }
                }
                else
                {
                    dateQuery = dateQuery.Replace("{1}", "12/31/2099 11:59:59 PM");
                }

                dt = Helper.DbHelper.ExecuteDataTable(repo.GetConnectionString(), string.Format(metric.query, query, dateQuery, additionalCondition, additionalCondition.Replace("'", "''")));
            }

            return dt;
        }

    }
}
