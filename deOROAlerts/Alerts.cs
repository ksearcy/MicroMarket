using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORODataAccess;
using ExcelLibrary;
using System.IO;

namespace deOROAlerts
{
    public class Alerts
    {
        private AlertRepository alertRepo = new AlertRepository();
        private MetricRepository metricRepo = new MetricRepository();
        private AlertSubscriptionRepository alertSubscriptionRepo = new AlertSubscriptionRepository();
        private LocationRepository locationRepo = new LocationRepository();

        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Email email = Email.Instance;

        public void ProcessSubscriptions()
        {
            var subscriptions = alertSubscriptionRepo.GetAll().ToList();
            int ExcelReportsCounter = 0;
            string ExcelReportsFilePaths = "";
            string ExcelReportsEmailHtml = "";
            string ExcelReportsEmailSubject = "";
            string ExcelReportsEmailContacts = "";
            string ExcelReportsSubscriptionIds = "";

            foreach (var subscription in subscriptions.Where(x => x.is_active == 1))
            {
                logger.Log(NLog.LogLevel.Info, string.Format("Start Processing {0}", subscription.id));
                var alert = alertRepo.FindBy(subscription.alertid.Value);

                try
                {
                    if (alert.next_run_date.Value.ToShortDateString() == DateTime.Now.ToShortDateString() || alert.frequeny == "UNDEFINED")
                    {
                        DateTime fromDate = DateTime.Now;
                        DateTime toDate = DateTime.Now;
                        DataTable dt = null;

                        if (alert.frequeny != "UNDEFINED")
                        {

                            if (alert.period == "YTD")
                            {
                                fromDate = new DateTime(DateTime.Now.Year, 1, 1);
                            }
                            else if (alert.period == "MTD")
                            {
                                fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                            }
                            else if (alert.period == "DTD")
                            {
                                fromDate = DateTime.Today;
                                toDate = DateTime.Today.AddHours(24);
                            }
                            else if (alert.period == "LastRunTimeTD")
                            {
                                fromDate = Convert.ToDateTime(alert.last_run_date);
                                toDate = DateTime.Today.AddHours(24);
                            }
                            else
                            {
                                try
                                {
                                    int period = int.Parse(alert.period);

                                    if (period > 0)
                                    {
                                        fromDate = DateTime.Now.AddDays(-period);
                                    }
                                }
                                catch
                                {
                                    throw new Exception("Invalid Period");
                                }
                            }

                            dt = GetMetricData(alert.metricid, subscription.customerid.Value,
                                                         subscription.locationid.Value, fromDate.ToString(),
                                                         toDate.ToString());



                            if (alert.report_type.Contains("Excel"))
                            {
                                // ======== EXCEL FILE POSITION INFORMATION =================
                                //If the DB cell report_type contains the start row and start column for the report it must be specified in the following format: "Excel 'RowNumber'-'ColumnNumber'".  EXAMPLE: "Excel 2-0" The report will start from the ROW #3 and the Column #1 
                                //If the DB cell report_type contains just the word "Excel" it will be start from ROW #1 and COLUMN #1 

                                string excelReportFilePath = "";

                                if (alert.report_type.Replace(" ", "") != "Excel")
                                {
                                    try
                                    {
                                        string[] RowAndColumnNumbers = alert.report_type.Replace("Excel ", "").Split('-');

                                        excelReportFilePath = Helper.CreateExcelReport(dt, alert.name, RowAndColumnNumbers[0], RowAndColumnNumbers[1]);
                                    }
                                    catch
                                    {

                                        excelReportFilePath = Helper.CreateExcelReport(dt, alert.name);
                                    }

                                }
                                else
                                {

                                    excelReportFilePath = Helper.CreateExcelReport(dt, alert.name);

                                }

                                if (System.Configuration.ConfigurationSettings.AppSettings["AllExcelReportsInOneEmail"] != "true" && subscriptions.Where(x => x.is_active == 1).Count() >= 1)
                                {

                                    string html = "<h1>" + alert.name + "<h1><br/>" + string.Format("<h3>Date Range: {0} - {1} <h3>", fromDate, toDate);

                                    string[] filesPathArray = excelReportFilePath.Split(',');

                                    email.SendEmail(subscription.email, "Auto Email - " + alert.name + " " + DateTime.Now.ToShortDateString(), html, "", filesPathArray);

                                    if (File.Exists(excelReportFilePath))
                                    {
                                        File.Delete(excelReportFilePath);
                                    }

                                    logger.Log(NLog.LogLevel.Info, string.Format("Report Sent {0}", subscription.id));

                                }
                                else
                                {

                                    if (ExcelReportsCounter != 0)
                                    {
                                        ExcelReportsFilePaths = ExcelReportsFilePaths + "," + excelReportFilePath;
                                        ExcelReportsEmailSubject = ExcelReportsEmailSubject + " & " + alert.name;
                                        if (ExcelReportsEmailContacts != subscription.email)
                                        {
                                            ExcelReportsEmailContacts = ExcelReportsEmailContacts + ", " + subscription.email;
                                        }

                                        ExcelReportsSubscriptionIds = ExcelReportsSubscriptionIds + "," + subscription.id.ToString();
                                    }
                                    else
                                    {
                                        ExcelReportsFilePaths = excelReportFilePath;
                                        ExcelReportsEmailSubject = alert.name;
                                        ExcelReportsEmailContacts = subscription.email;
                                        ExcelReportsSubscriptionIds = subscription.id.ToString();
                                    }
                                    ExcelReportsEmailHtml = ExcelReportsEmailHtml + "<h1>" + alert.name + "<h1>  " + string.Format("<h3>Date Range: {0} - {1} <h3><br/>", fromDate, toDate);
                                    ExcelReportsCounter += 1;



                                }


                            }

                            else if (alert.report_type.Contains("Email"))
                            {
                                string html = Helper.FormatHTML(dt, fromDate.ToString(), toDate.ToString(), alert.name);
                                email.SendEmail(subscription.email, "Auto Email - " + alert.name, html);
                                logger.Log(NLog.LogLevel.Info, string.Format("Alert Sent {0}", subscription.id));
                            }
                            else if (alert.report_type.Contains("TextFile"))
                            {

                                int i = 0;
                                StreamWriter sw = null;

                                string path = @"C:\Users\deoro\Documents\ARCAReport.txt";

                                if (File.Exists(path))
                                {
                                    File.Delete(path);
                                }

                                sw = new StreamWriter(path, false);

                                for (i = 0; i < dt.Columns.Count - 1; i++)
                                {

                                    sw.Write(dt.Columns[i].ColumnName + "|");

                                }
                                sw.Write(dt.Columns[i].ColumnName);
                                sw.WriteLine();

                                foreach (DataRow row in dt.Rows)
                                {
                                    object[] array = row.ItemArray;

                                    for (i = 0; i < array.Length - 1; i++)
                                    {
                                        sw.Write(array[i].ToString() + "|");
                                    }
                                    sw.Write(array[i].ToString());
                                    sw.WriteLine();

                                }

                                sw.Close();

                            }


                        }
                        else
                        {

                            if (alert.report_type.Contains("ServicedRouteLocations"))
                            {
                                DataTable servicedRoutes = locationRepo.GetServicedRoutes();


                                foreach (DataRow rowRoutes in servicedRoutes.Rows)
                                {
                                    object[] arrayRoutes = rowRoutes.ItemArray;

                                    if (dt != null)
                                    {
                                        dt.Clear(); 
                                    }
                                    
                                    
                                    dt = GetMetricData(alert.metricid, subscription.customerid.Value,
                                                       subscription.locationid.Value, fromDate.ToString(),
                                                       toDate.ToString(), arrayRoutes[0].ToString());


                                    if (alert.report_type.Contains("TextFile"))
                                    {

                                        int i = 0;
                                        StreamWriter sw = null;

                                        string RouteFormated = arrayRoutes[0].ToString();

                                        switch (RouteFormated.Length)
                                        {
                                            case 1:
                                             RouteFormated= "000" + RouteFormated;
                                                break;
                                            case 2:
                                             RouteFormated = "00" + RouteFormated;
                                                break;
                                            case 3:
                                             RouteFormated = "0" + RouteFormated;
                                                break;
                                            default:
                                                break;
                                        }

                                        string path = @System.Configuration.ConfigurationSettings.AppSettings["TxtReportsPath"].ToString() + alert.name + RouteFormated + DateTime.Today.ToString("yyMMdd") + ".txt";

                                        if (File.Exists(path))
                                        {
                                            File.Delete(path);
                                        }

                                        sw = new StreamWriter(path, false);

                                        for (i = 0; i < dt.Columns.Count - 1; i++)
                                        {

                                            sw.Write(dt.Columns[i].ColumnName + "|");

                                        }
                                        sw.Write(dt.Columns[i].ColumnName);
                                        sw.WriteLine();

                                        foreach (DataRow row in dt.Rows)
                                        {
                                            object[] array = row.ItemArray;

                                            for (i = 0; i < array.Length - 1; i++)
                                            {
                                                sw.Write(array[i].ToString() + "|");
                                            }
                                            sw.Write(array[i].ToString());
                                            sw.WriteLine();

                                        }

                                        sw.Close();

                                    }
                                
                                }               

                            }

                        }
                    }
                    else
                    {
                        logger.Log(NLog.LogLevel.Info, string.Format("Alert Not Sent {0} - Next Run Date {1}", subscription.id, alert.next_run_date));
                    }

                    logger.Log(NLog.LogLevel.Info, string.Format("End Processing {0} \r\n", subscription.id));
                }
                catch (Exception ex)
                {
                    logger.Log(NLog.LogLevel.Error, alert.id + "--------------------------");
                    logger.Log(NLog.LogLevel.Error, ex.ToString() + "\r\n");
                }

            }

            if (System.Configuration.ConfigurationSettings.AppSettings["AllExcelReportsInOneEmail"] == "true" && ExcelReportsCounter > 0)
            {

                string[] filesPathArray = ExcelReportsFilePaths.Split(',');

                string[] subscriptionsIds = ExcelReportsSubscriptionIds.Split(',');



                email.SendEmail(ExcelReportsEmailContacts, ExcelReportsEmailSubject + " " + DateTime.Now.ToShortDateString(), ExcelReportsEmailHtml, "", filesPathArray);

                foreach (string s in subscriptionsIds)
                {
                    logger.Log(NLog.LogLevel.Info, string.Format("Report Sent {0}", s));
                }

            }

        }

        public void SetNextRunDates()
        {
            var alerts = alertRepo.GetAll().ToList();

            foreach (var alert in alerts)
            {
                if (alert.next_run_date.Value.ToShortDateString() == DateTime.Now.ToShortDateString())
                {
                    alert.last_run_date = DateTime.Now;

                    if (alert.frequency == "D")
                    {
                        alert.next_run_date = DateTime.Now.AddDays(1);
                    }
                    else if (alert.frequency == "W")
                    {
                        alert.next_run_date = DateTime.Now.AddDays(7);
                    }
                    else if (alert.frequency == "M")
                    {
                        alert.next_run_date = DateTime.Now.AddMonths(1);
                    }
                    else if (alert.frequency == "Y")
                    {
                        alert.next_run_date = DateTime.Now.AddYears(1);
                    }

                    alertRepo.Edit(alert);
                }
            }

            alertRepo.Save();
        }

        private DataTable GetMetricData(int id, int customerid, int? locationid, string fromdate, string todate, string customvalue = null)
        {
            DataTable dt = new DataTable();
            var metric = metricRepo.GetSingleById(x => x.id == id);

            if (metric != null)
            {
                string query = "";
                string dateQuery = metric.date_range;

                if (customvalue == null)
                {
                    if (dateQuery != null)
                    {
                        if (customerid != -1)
                            query += " AND o.customerid = " + customerid;

                        if (locationid != null && locationid != -1)
                            query += " AND o.locationid = " + locationid;

                        if (fromdate != "")
                            dateQuery = dateQuery.Replace("{0}", fromdate);
                        else
                            dateQuery = dateQuery.Replace("{0}", "1/1/1900");

                        if (todate != "")
                            dateQuery = dateQuery.Replace("{1}", todate);
                        else
                            dateQuery = dateQuery.Replace("{1}", "12/31/2099");

                        dt = Helper.ExecuteDataTable(metricRepo.GetConnectionString(), string.Format(metric.query, query, dateQuery));
                    }
                    else
                    {

                        if (metric.query.Contains("{0}") && metric.query.Contains("{1}"))
                        {
                            if (fromdate == "")
                                fromdate = "1/1/1900";

                            if (todate == "")
                                todate = "12/31/2099";

                            metric.query = metric.query.Replace("{0}", fromdate);
                            metric.query = metric.query.Replace("{1}", todate);
                        }

                        if (metric.query.Contains("{2}"))
                        {
                            if (customerid != -1)
                                query += " AND o.customerid = " + customerid;

                            if (locationid != null && locationid != -1)
                                query += " AND o.locationid = " + locationid;

                            metric.query = metric.query.Replace("{2}", query);
                        }

                        dt = Helper.ExecuteDataTable(metricRepo.GetConnectionString(), metric.query);

                    }

                }
                else
                {

                    if (dateQuery != null)
                    {

                        dateQuery = dateQuery.Replace("{0}", customvalue);

                        dt = Helper.ExecuteDataTable(metricRepo.GetConnectionString(), string.Format(metric.query, dateQuery));
                    }

                }


            }

            return dt;
        }

    }
}
