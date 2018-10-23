using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using deORODataAccess;
using deORODataAccess.DTO;
using deOROWeb.Helper;

namespace deOROWeb.Controllers
{
    //[deOROWeb.Security.deORORoleAuthorize(Roles = "Admin")]
    public class ReportsController : MyBaseController
    {
        MetricRepository metric = new MetricRepository();
        LocationItemRepository locationItemRepo = new LocationItemRepository();
        LocationServiceRepository locationServiceRepo = new LocationServiceRepository();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Sales()
        {
            CustomerRepository customer = new CustomerRepository();
            ViewBag.Customers = customer.GetAll();

            return View();
        }

        public ActionResult MachineRefillSales()
        {
            CustomerRepository customer = new CustomerRepository();
            ViewBag.Customers = customer.GetAll();

            return View();
        }


        public ActionResult Errors()
        {
            CustomerRepository customer = new CustomerRepository();
            ViewBag.Customers = customer.GetAll();

            return View();
        }

        public ActionResult CashAccountability()
        {
            CustomerRepository customer = new CustomerRepository();
            ViewBag.Customers = customer.GetAll();

            return View();
        }

        public ActionResult Profit()
        {
            CustomerRepository customer = new CustomerRepository();
            ViewBag.Customers = customer.GetAll();

            return View();
        }

        public ActionResult Tax()
        {
            CustomerRepository customer = new CustomerRepository();
            ViewBag.Customers = customer.GetAll();

            return View();
        }

        public ActionResult UserTransactions()
        {
            CustomerRepository customer = new CustomerRepository();
            ViewBag.Customers = customer.GetAll();

            return View();
        }

        public ActionResult OverUnder()
        {
            CustomerRepository customer = new CustomerRepository();
            ViewBag.Customers = customer.GetAll();

            return View();
        }

        public ActionResult CustomerPayment()
        {
            CustomerRepository customer = new CustomerRepository();
            ViewBag.Customers = customer.GetAll();

            return View();
        }

        public dynamic ProcessReport(int customerid, int[] locationids, string reportCategory, string reportType, string reportView,
                                     string fromDate, string toDate, string fromHour, string toHour, string fromDaytime, string toDaytime)
        {

            if (reportCategory != "MachineRefillSales")
            {
                fromDate = fromDate + " " + fromHour + ":00:00" + " " + fromDaytime;
                toDate = toDate + " " + toHour + ":00:00" + " " + toDaytime;
            }


            MetricController controller = new MetricController();
            DataTable dt = new DataTable();

            bool showTotals = true;
            bool showSparkLines = false;
            int groupColumn = 0;

            List<object> chartData = new List<object>();

            if (reportCategory == "Sales" )
            {
                if (reportType == "Items")
                {
                    if (reportView.Equals("Default"))
                    {
                        showSparkLines = true;
                        dt = controller.GetMetricData(9, customerid, locationids, fromDate, toDate);

                        foreach (DataRow row in dt.Rows)
                        {
                            row["Last 30 Day Sales"] = locationItemRepo.GetSalesData(Convert.ToInt32(row["LocationId"]), Convert.ToInt32(row["ItemId"]), 30);
                        }

                        dt.Columns.Remove("LocationId");
                        dt.Columns.Remove("ItemId");
                    }
                    else if (reportView.Equals("Hourly"))
                    {
                        showTotals = false;
                        dt = controller.GetMetricData(15, customerid, locationids, fromDate, toDate);

                        chartData = PrepareChartObject(dt);
                    }
                    else if (reportView.Equals("Day of Week"))
                    {
                        showTotals = false;
                        dt = controller.GetMetricData(18, customerid, locationids, fromDate, toDate);

                        foreach (DataColumn dc in dt.Columns)
                        {
                            switch (dc.ColumnName)
                            {
                                case "1":
                                    {
                                        dc.ColumnName = "Sunday";
                                        break;
                                    }
                                case "2":
                                    {
                                        dc.ColumnName = "Monday";
                                        break;
                                    }
                                case "3":
                                    {
                                        dc.ColumnName = "Tuesday";
                                        break;
                                    }
                                case "4":
                                    {
                                        dc.ColumnName = "Wednesday";
                                        break;
                                    }
                                case "5":
                                    {
                                        dc.ColumnName = "Thursday";
                                        break;
                                    }
                                case "6":
                                    {
                                        dc.ColumnName = "Friday";
                                        break;
                                    }
                                case "7":
                                    {
                                        dc.ColumnName = "Saturday";
                                        break;
                                    }
                            }
                        }

                        //if (dt.Rows.Count > 0)
                        //{
                        //    items = from b in dt.Transpose().AsEnumerable()
                        //            select new object[] 
                        //            {
                        //                b.Field<string>(0),
                        //                b.Field<string>(1)
                        //            };
                        //}

                        chartData = PrepareChartObject(dt);
                    }
                    else if (reportView.Equals("Daily"))
                    {
                        showTotals = false;
                        dt = controller.GetMetricData(16, customerid, locationids, fromDate, toDate);

                        //if (dt.Rows.Count > 0)
                        //{
                        //    items = from b in dt.Transpose().AsEnumerable()
                        //            select new object[] 
                        //            {
                        //                b.Field<string>(0),
                        //                b.Field<string>(1)
                        //            };
                        //}

                        chartData = PrepareChartObject(dt);
                    }
                    else if (reportView.Equals("Weekly"))
                    {
                        dt = controller.GetMetricData(12, customerid, locationids, fromDate, toDate);

                        string computeString = "";

                        for (int i = 3; i < dt.Columns.Count; i++)
                        {

                            if (!(fromDate == "" || toDate == ""))
                            {
                                //if (Convert.ToDateTime(fromDate).Year == Convert.ToDateTime(toDate).Year)
                                //{
                                //    DateTime firstDay = FirstDateOfWeek(Convert.ToDateTime(toDate).Year, Convert.ToInt32(dt.Columns[i].ColumnName), CultureInfo.CurrentCulture);
                                //    DateTime lastDay = firstDay.AddDays(6);

                                //    dt.Columns[i].ColumnName = string.Format("Week {0} ({1} - {2})", dt.Columns[i].ColumnName, firstDay.ToString("d"), lastDay.ToString("d"));
                                //}
                                //else
                                //{
                                //    dt.Columns[i].ColumnName = string.Format("Week {0}", dt.Columns[i].ColumnName);
                                //}

                                dt.Columns[i].ColumnName = string.Format("{0}", dt.Columns[i].ColumnName);
                            }
                            else
                            {
                                dt.Columns[i].ColumnName = string.Format("{0}", dt.Columns[i].ColumnName);
                            }

                            computeString += "[" + dt.Columns[i].ColumnName + "] +";
                        }

                        computeString = computeString.Substring(0, computeString.Length - 1);

                        dt.Columns.Add("# Sales", typeof(int), computeString);
                        dt.Columns.Add("$ Sales", typeof(decimal), "[# Sales] * [Price]");


                        DataTable dtWeekly = new DataTable();
                        dtWeekly.Columns.Add("Location");

                        for (int i = 1; i <= 53; i++)
                        {
                            dtWeekly.Columns.Add(i.ToString());
                            System.Diagnostics.Debug.WriteLine(i);
                        }

                        var weekly = (from d in dt.AsEnumerable()
                                      group d by d.Field<string>("Location") into g
                                      select dtWeekly.LoadDataRow(
                                      new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["1"] != null ? x.Field<int>("1") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["2"] != null ? x.Field<int>("2") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["3"] != null ? x.Field<int>("3") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["4"] != null ? x.Field<int>("4") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["5"] != null ? x.Field<int>("5") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["6"] != null ? x.Field<int>("6") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["7"] != null ? x.Field<int>("7") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["8"] != null ? x.Field<int>("8") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["9"] != null ? x.Field<int>("9") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["10"] != null ? x.Field<int>("10") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["11"] != null ? x.Field<int>("11") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["12"] != null ? x.Field<int>("12") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["13"] != null ? x.Field<int>("13") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["14"] != null ? x.Field<int>("14") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["15"] != null ? x.Field<int>("15") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["16"] != null ? x.Field<int>("16") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["17"] != null ? x.Field<int>("17") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["18"] != null ? x.Field<int>("18") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["19"] != null ? x.Field<int>("19") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["20"] != null ? x.Field<int>("20") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["21"] != null ? x.Field<int>("21") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["22"] != null ? x.Field<int>("22") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["23"] != null ? x.Field<int>("23") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["24"] != null ? x.Field<int>("24") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["25"] != null ? x.Field<int>("25") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["26"] != null ? x.Field<int>("26") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["27"] != null ? x.Field<int>("27") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["28"] != null ? x.Field<int>("28") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["29"] != null ? x.Field<int>("29") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["30"] != null ? x.Field<int>("30") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["31"] != null ? x.Field<int>("31") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["32"] != null ? x.Field<int>("32") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["33"] != null ? x.Field<int>("33") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["34"] != null ? x.Field<int>("34") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["35"] != null ? x.Field<int>("35") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["36"] != null ? x.Field<int>("36") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["37"] != null ? x.Field<int>("37") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["38"] != null ? x.Field<int>("38") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["39"] != null ? x.Field<int>("39") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["40"] != null ? x.Field<int>("40") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["41"] != null ? x.Field<int>("41") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["42"] != null ? x.Field<int>("42") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["43"] != null ? x.Field<int>("43") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["44"] != null ? x.Field<int>("44") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["45"] != null ? x.Field<int>("45") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["46"] != null ? x.Field<int>("46") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["47"] != null ? x.Field<int>("47") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["48"] != null ? x.Field<int>("48") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["49"] != null ? x.Field<int>("49") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["50"] != null ? x.Field<int>("50") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["51"] != null ? x.Field<int>("51") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["52"] != null ? x.Field<int>("52") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["53"] != null ? x.Field<int>("53") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtWeekly);

                    }
                    else if (reportView.Equals("Monthly"))
                    {
                        showTotals = true;
                        dt = controller.GetMetricData(13, customerid, locationids, fromDate, toDate);

                        string computeString = "";

                        for (int i = 3; i < dt.Columns.Count; i++)
                        {
                            dt.Columns[i].ColumnName = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(Convert.ToInt32(dt.Columns[i].ColumnName));
                            computeString += "[" + dt.Columns[i].ColumnName + "] +";
                        }

                        computeString = computeString.Substring(0, computeString.Length - 1);

                        dt.Columns.Add("# Sales", typeof(int), computeString);
                        dt.Columns.Add("$ Sales", typeof(decimal), "[# Sales] * [Price]");

                        DataTable dtMonthly = new DataTable();
                        dtMonthly.Columns.Add("Location");
                        dtMonthly.Columns.Add("Jan");
                        dtMonthly.Columns.Add("Feb");
                        dtMonthly.Columns.Add("Mar");
                        dtMonthly.Columns.Add("Apr");
                        dtMonthly.Columns.Add("May");
                        dtMonthly.Columns.Add("Jun");
                        dtMonthly.Columns.Add("Jul");
                        dtMonthly.Columns.Add("Aug");
                        dtMonthly.Columns.Add("Sep");
                        dtMonthly.Columns.Add("Oct");
                        dtMonthly.Columns.Add("Nov");
                        dtMonthly.Columns.Add("Dec");

                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtMonthly.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["Jan"] != null ? x.Field<int>("Jan") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Feb"] != null ? x.Field<int>("Feb") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Mar"] != null ? x.Field<int>("Mar") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Apr"] != null ? x.Field<int>("Apr") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["May"] != null ? x.Field<int>("May") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Jun"] != null ? x.Field<int>("May") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Jul"] != null ? x.Field<int>("May") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Aug"] != null ? x.Field<int>("Aug") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Sep"] != null ? x.Field<int>("Sep") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Oct"] != null ? x.Field<int>("Oct") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Nov"] != null ? x.Field<int>("Nov") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Dec"] != null ? x.Field<int>("Dec") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtMonthly);
                    }

                    //showCharts = true;
                    //chartData = PrepareChartObject(dt);


                }
                else if (reportType == "PaymentMethodByItemCount")
                {
                    DataTable dtPayment = new DataTable();
                    dtPayment.Columns.Add("Location");

                    if (reportView.Equals("Pay"))
                    {
                        dt = controller.GetMetricData(19, customerid, locationids, fromDate, toDate, "AND  Source Like '%Pay'");

                        dtPayment.Columns.Add("BillPay");
                        dtPayment.Columns.Add("CoinPay");
                        dtPayment.Columns.Add("CreditCardPay");
                        dtPayment.Columns.Add("MyAccountPay");
                        dtPayment.Columns.Add("MyPayrollPay");

                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["BillPay"] != null ? x.Field<int?>("BillPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinPay"] != null ? x.Field<int?>("CoinPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardPay"] != null ? x.Field<int?>("CreditCardPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["MyAccountPay"] != null ? x.Field<int?>("MyAccountPay") : 0;}),
                                         g.Sum(x => { return x.Table.Columns["MyPayrollPay"] != null ? x.Field<int?>("MyPayrollPay") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);

                    }
                    else if (reportView.Equals("Refill"))
                    {
                        //dt = controller.GetMetricData(5, customerid, locationids, fromDate, toDate, "AND Source Like '%Refill'");

                        dt = controller.GetMetricData(19, customerid, locationids, fromDate, toDate, "AND  Source Like '%Refill'");

                        dtPayment.Columns.Add("BillRefill");
                        dtPayment.Columns.Add("CoinRefill");
                        dtPayment.Columns.Add("CreditCardRefill");

                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["BillRefill"] != null ? x.Field<int?>("BillRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinRefill"] != null ? x.Field<int?>("CoinRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardRefill"] != null ? x.Field<int?>("CreditCardRefill") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);

                    }
                    else
                    {
                        dt = controller.GetMetricData(19, customerid, locationids, fromDate, toDate, "AND Source IN ('BillPay','BillRefill','CoinPay','CoinRefill','CreditCardPay','CreditCardRefill','MyAccountPay','MyPayrollPay')");

                        dtPayment.Columns.Add("BillPay");
                        dtPayment.Columns.Add("BillRefill");
                        dtPayment.Columns.Add("CoinPay");
                        dtPayment.Columns.Add("CoinRefill");
                        dtPayment.Columns.Add("CreditCardPay");
                        dtPayment.Columns.Add("CreditCardRefill");
                        dtPayment.Columns.Add("MyAccountPay");
                        dtPayment.Columns.Add("MyPayrollPay");


                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["BillPay"] != null ? x.Field<int?>("BillPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["BillRefill"] != null ? x.Field<int?>("BillRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinPay"] != null ? x.Field<int?>("CoinPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinRefill"] != null ? x.Field<int?>("CoinRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardPay"] != null ? x.Field<int?>("CreditCardPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardRefill"] != null ? x.Field<int?>("CreditCardRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["MyAccountPay"] != null ? x.Field<int?>("MyAccountPay") : 0;}),
                                          g.Sum(x => { return x.Table.Columns["MyPayrollPay"] != null ? x.Field<int?>("MyPayrollPay") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);


                    }
                }
                else if (reportType == "NoSales")
                {
                    dt = controller.GetMetricData(1, customerid, locationids, fromDate, toDate);
                }
                else if (reportType == "PaymentRouting")
                {
                    dt = controller.GetMetricData(31, customerid, locationids, fromDate, toDate);
                }
                else if (reportType == "PaymentMethodByAmount")
                {
                    DataTable dtPayment = new DataTable();
                    dtPayment.Columns.Add("Location");

                    if (reportView.Equals("Pay"))
                    {
                        dt = controller.GetMetricData(27, customerid, locationids, fromDate, toDate, "AND  Source Like '%Pay'");

                        dtPayment.Columns.Add("BillPay");
                        dtPayment.Columns.Add("CoinPay");
                        dtPayment.Columns.Add("CreditCardPay");
                        dtPayment.Columns.Add("MyAccountPay");
                        dtPayment.Columns.Add("MyPayrollPay");

                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["BillPay"] != null ? x.Field<decimal?>("BillPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinPay"] != null ? x.Field<decimal?>("CoinPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardPay"] != null ? x.Field<decimal?>("CreditCardPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["MyAccountPay"] != null ? x.Field<decimal?>("MyAccountPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["MyPayrollPay"] != null ? x.Field<decimal?>("MyPayrollPay") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);

                    }
                    else if (reportView.Equals("Refill"))
                    {
                        dt = controller.GetMetricData(27, customerid, locationids, fromDate, toDate, "AND  Source Like '%Refill'");

                        dtPayment.Columns.Add("BillRefill");
                        dtPayment.Columns.Add("CoinRefill");
                        dtPayment.Columns.Add("CreditCardRefill");

                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["BillRefill"] != null ? x.Field<decimal?>("BillRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinRefill"] != null ? x.Field<decimal?>("CoinRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardRefill"] != null ? x.Field<decimal?>("CreditCardRefill") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);

                    }
                    else
                    {
                        dt = controller.GetMetricData(27, customerid, locationids, fromDate, toDate, "AND Source IN ('BillPay','BillRefill','CoinPay','CoinRefill','CreditCardPay','CreditCardRefill','MyAccountPay','MyPayrollPay','Purchase Complete')");

                        dtPayment.Columns.Add("BillPay");
                        dtPayment.Columns.Add("BillRefill");
                        dtPayment.Columns.Add("CoinPay");
                        dtPayment.Columns.Add("CoinRefill");
                        dtPayment.Columns.Add("CreditCardPay");
                        dtPayment.Columns.Add("CreditCardRefill");
                        dtPayment.Columns.Add("MyAccountPay");
                        dtPayment.Columns.Add("MyPayrollPay");
                        //dtPayment.Columns.Add("Purchase Complete MyAccountPay");


                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["BillPay"] != null ? x.Field<decimal?>("BillPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["BillRefill"] != null ? x.Field<decimal?>("BillRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinPay"] != null ? x.Field<decimal?>("CoinPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinRefill"] != null ? x.Field<decimal?>("CoinRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardPay"] != null ? x.Field<decimal?>("CreditCardPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardRefill"] != null ? x.Field<decimal?>("CreditCardRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["MyAccountPay"] != null ? x.Field<decimal?>("MyAccountPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["MyPayrollPay"] != null ? x.Field<decimal?>("MyPayrollPay") : 0;}),
                                        //g.Sum(x => { return x.Table.Columns["Purchase Complete MyAccountPay"] != null ? x.Field<decimal?>("Purchase Complete MyAccountPay") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);


                    }

                }
                //======================New Payroll Report==================
                else if (reportType == "Payroll")
                {
                    DataTable dtPayment = new DataTable();
                    dtPayment.Columns.Add("Location");

                    if (reportView.Equals("Pay"))
                    {
                        dt = controller.GetMetricData(27, customerid, locationids, fromDate, toDate, "AND  Source Like '%Pay'");


                        dtPayment.Columns.Add("MyPayrollPay");

                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,                                       
                                        g.Sum(x => { return x.Table.Columns["MyPayrollPay"] != null ? x.Field<decimal?>("MyPayrollPay") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);

                    }
                    else if (reportView.Equals("Refill"))
                    {
                        dt = controller.GetMetricData(27, customerid, locationids, fromDate, toDate, "AND  Source Like '%Refill'");

                        dtPayment.Columns.Add("BillRefill");
                        dtPayment.Columns.Add("CoinRefill");
                        dtPayment.Columns.Add("CreditCardRefill");

                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["BillRefill"] != null ? x.Field<decimal?>("BillRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinRefill"] != null ? x.Field<decimal?>("CoinRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardRefill"] != null ? x.Field<decimal?>("CreditCardRefill") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);

                    }
                    else if (reportView.Equals("Users"))
                    {
                        dt = controller.GetMetricData(33, customerid, locationids, fromDate, toDate);
                    }
                    else
                    {
                        dt = controller.GetMetricData(27, customerid, locationids, fromDate, toDate, "AND Source IN ('MyPayrollPay')");


                        dtPayment.Columns.Add("MyPayrollPay");


                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                       
                                        g.Sum(x => { return x.Table.Columns["MyPayrollPay"] != null ? x.Field<decimal?>("MyPayrollPay") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);


                    }


                }
                //==========================================================
                else if (reportType == "ShoppingCartDetail")
                {
                    if (reportView == "Default")
                    {
                        dt = controller.GetMetricData(14, customerid, locationids, fromDate, toDate);
                    }
                    else
                    {
                        dt = controller.GetMetricData(24, customerid, locationids, fromDate, toDate);
                        groupColumn = 3;
                        dt.Columns[0].ColumnName = " ";
                    }
                }
                else if (reportType == "Inventory")
                {
                    if (reportView == "Default")
                    {
                        dt = controller.GetMetricData(32, customerid, locationids, fromDate, toDate);
                    }
                    if (reportView == "TotalRefill")
                    {
                        dt = controller.GetMetricData(34, customerid, locationids, fromDate, toDate);
                    }

                }
            }
            else if (reportCategory == "CustomerPayment")
            {
                dt = controller.GetMetricData(29, customerid, locationids, fromDate, toDate);
            }
            else if (reportCategory == "CashAccountability")
            {
                if (reportType == "CashCollection")
                {
                    dt = controller.GetMetricData(10, customerid, locationids, fromDate, toDate);

                    //string computeTotal = @"(Coins) +  (1*[$1s]) + (2*[$2s]) + (5*[$5s]) + (10*[$10s]) + (20*[$20s]) + (50*[$50s]) + (100*[$100s])";

                    //dt.Columns["Total"].Expression = computeTotal;
                    dt.Columns["Total Deficit"].Expression = "[Cash Total] - [Recon Total]";
                    dt.Columns[0].ColumnName = " ";
                }
                else if (reportType == "CashFlow") {

                    dt = controller.GetMetricData(35, customerid, locationids, fromDate, toDate);

                    //string computeTotal = @"(Coins) +  (1*[$1s]) + (2*[$2s]) + (5*[$5s]) + (10*[$10s]) + (20*[$20s]) + (50*[$50s]) + (100*[$100s])";

                    //dt.Columns["Total"].Expression = computeTotal;
                    //dt.Columns["Total Deficit"].Expression = "[Cash Total] - [Recon Total]";
                   // dt.Columns[0].ColumnName = " ";
                
                }
            }
            else if (reportCategory == "OverUnder")
            {
                if (reportType == "ItemTotal")
                {
                    dt = controller.GetMetricData(28, customerid, locationids, fromDate, toDate);
                }
            }
            else if (reportCategory == "Profit")
            {
                if (reportType == "TotalProfit")
                {
                    dt = controller.GetMetricData(11, customerid, locationids, fromDate, toDate);
                }
            }
            else if (reportCategory == "Tax")
            {
                if (reportType == "Tax")
                {
                    if (reportView.Equals("Default"))
                    {
                        dt = controller.GetMetricData(17, customerid, locationids, fromDate, toDate);
                    }
                    else if (reportView.Equals("Category"))
                    {
                        dt = controller.GetMetricData(26, customerid, locationids, fromDate, toDate);
                    }
                }
            }
            else if (reportCategory == "UserTransactions")
            {
                if (reportType == "AccountBalance")
                {
                    dt = controller.GetMetricData(8, customerid, locationids, fromDate, toDate);
                }
                else if (reportType == "Transactions")
                {
                    dt = controller.GetMetricData(25, customerid, locationids, fromDate, toDate);
                }

                groupColumn = 2;
                dt.Columns[0].ColumnName = " ";
            }
            else if (reportCategory == "Errors")
            {
                DataTable dtErrors = new DataTable();
                dtErrors.Columns.Add("Location");

                if (reportType == "Transaction")
                {
                    if (reportView == "Default")
                    {
                        dt = controller.GetMetricData(20, customerid, locationids, fromDate, toDate);
                    }
                    else if (reportView == "Count")
                    {
                        dtErrors.Columns.Add("BarcodeScanner");
                        dtErrors.Columns.Add("Bill");
                        dtErrors.Columns.Add("Coin");
                        dtErrors.Columns.Add("CreditCardReader");

                        dt = controller.GetMetricData(22, customerid, locationids, fromDate, toDate);


                        var errors = (from d in dt.AsEnumerable()
                                      group d by d.Field<string>("Location") into g
                                      select dtErrors.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["BarcodeScanner"] != null ? x.Field<int?>("BarcodeScanner") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Bill"] != null ? x.Field<int?>("Bill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Coin"] != null ? x.Field<int?>("Coin") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardReader"] != null ? x.Field<int?>("CreditCardReader") : 0;})
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtErrors);
                    }

                    showTotals = false;
                }
                else if (reportType == "Device")
                {
                    if (reportView == "Default")
                    {
                        dt = controller.GetMetricData(21, customerid, locationids, fromDate, toDate);
                    }
                    else if (reportView == "Count")
                    {
                        dtErrors.Columns.Add("BarcodeScanner");
                        dtErrors.Columns.Add("CreditCardReader");

                        dt = controller.GetMetricData(23, customerid, locationids, fromDate, toDate);

                        if (dt.Columns["BarcodeScanner"] == null)
                            dt.Columns.Add("BarcodeScanner");

                        if (dt.Columns["CreditCardReader"] == null)
                            dt.Columns.Add("CreditCardReader");


                        var errors = (from d in dt.AsEnumerable()
                                      group d by d.Field<string>("Location") into g
                                      select dtErrors.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["BarcodeScanner"] != null ? x.Field<int?>("BarcodeScanner") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardReader"] != null ? x.Field<int?>("CreditCardReader") : 0;})
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtErrors);
                    }

                    showTotals = false;
                }
            }
//==============================================================================DEVELOPMENT SECTION=========================================================================
            else if (reportCategory == "MachineRefillSales")
            {
                
                //==============Convert Serviceid(fromDate varaible ) to the last service before the selected service             
                //string PreviousServiceDate = (Convert.ToInt32(fromDate) - 1);             
                fromDate = GetPreviousServiceDate(toDate, locationids[0], customerid);
                //=======================================================================================================================        

                if (reportType == "CashCollection")
                {
                    dt = controller.GetMetricData(10, customerid, locationids, fromDate, toDate);

                    //string computeTotal = @"(Coins) +  (1*[$1s]) + (2*[$2s]) + (5*[$5s]) + (10*[$10s]) + (20*[$20s]) + (50*[$50s]) + (100*[$100s])";

                    //dt.Columns["Total"].Expression = computeTotal;
                    dt.Columns["Total Deficit"].Expression = "[Cash Total] - [Recon Total]";
                    dt.Columns[0].ColumnName = " ";
                }
                else if (reportType == "Items")
                {
                    if (reportView.Equals("Default"))
                    {
                        showSparkLines = true;
                        dt = controller.GetMetricData(9, customerid, locationids, fromDate, toDate);

                        foreach (DataRow row in dt.Rows)
                        {
                            row["Last 30 Day Sales"] = locationItemRepo.GetSalesData(Convert.ToInt32(row["LocationId"]), Convert.ToInt32(row["ItemId"]), 30);
                        }

                        dt.Columns.Remove("LocationId");
                        dt.Columns.Remove("ItemId");
                    }
                    else if (reportView.Equals("Hourly"))
                    {
                        showTotals = false;
                        dt = controller.GetMetricData(15, customerid, locationids, fromDate, toDate);

                        chartData = PrepareChartObject(dt);
                    }
                    else if (reportView.Equals("Day of Week"))
                    {
                        showTotals = false;
                        dt = controller.GetMetricData(18, customerid, locationids, fromDate, toDate);

                        foreach (DataColumn dc in dt.Columns)
                        {
                            switch (dc.ColumnName)
                            {
                                case "1":
                                    {
                                        dc.ColumnName = "Sunday";
                                        break;
                                    }
                                case "2":
                                    {
                                        dc.ColumnName = "Monday";
                                        break;
                                    }
                                case "3":
                                    {
                                        dc.ColumnName = "Tuesday";
                                        break;
                                    }
                                case "4":
                                    {
                                        dc.ColumnName = "Wednesday";
                                        break;
                                    }
                                case "5":
                                    {
                                        dc.ColumnName = "Thursday";
                                        break;
                                    }
                                case "6":
                                    {
                                        dc.ColumnName = "Friday";
                                        break;
                                    }
                                case "7":
                                    {
                                        dc.ColumnName = "Saturday";
                                        break;
                                    }
                            }
                        }

                        //if (dt.Rows.Count > 0)
                        //{
                        //    items = from b in dt.Transpose().AsEnumerable()
                        //            select new object[] 
                        //            {
                        //                b.Field<string>(0),
                        //                b.Field<string>(1)
                        //            };
                        //}

                        chartData = PrepareChartObject(dt);
                    }
                    else if (reportView.Equals("Daily"))
                    {
                        showTotals = false;
                        dt = controller.GetMetricData(16, customerid, locationids, fromDate, toDate);

                        //if (dt.Rows.Count > 0)
                        //{
                        //    items = from b in dt.Transpose().AsEnumerable()
                        //            select new object[] 
                        //            {
                        //                b.Field<string>(0),
                        //                b.Field<string>(1)
                        //            };
                        //}

                        chartData = PrepareChartObject(dt);
                    }
                    else if (reportView.Equals("Weekly"))
                    {
                        dt = controller.GetMetricData(12, customerid, locationids, fromDate, toDate);

                        string computeString = "";

                        for (int i = 3; i < dt.Columns.Count; i++)
                        {

                            if (!(fromDate == "" || toDate == ""))
                            {
                                //if (Convert.ToDateTime(fromDate).Year == Convert.ToDateTime(toDate).Year)
                                //{
                                //    DateTime firstDay = FirstDateOfWeek(Convert.ToDateTime(toDate).Year, Convert.ToInt32(dt.Columns[i].ColumnName), CultureInfo.CurrentCulture);
                                //    DateTime lastDay = firstDay.AddDays(6);

                                //    dt.Columns[i].ColumnName = string.Format("Week {0} ({1} - {2})", dt.Columns[i].ColumnName, firstDay.ToString("d"), lastDay.ToString("d"));
                                //}
                                //else
                                //{
                                //    dt.Columns[i].ColumnName = string.Format("Week {0}", dt.Columns[i].ColumnName);
                                //}

                                dt.Columns[i].ColumnName = string.Format("{0}", dt.Columns[i].ColumnName);
                            }
                            else
                            {
                                dt.Columns[i].ColumnName = string.Format("{0}", dt.Columns[i].ColumnName);
                            }

                            computeString += "[" + dt.Columns[i].ColumnName + "] +";
                        }

                        computeString = computeString.Substring(0, computeString.Length - 1);

                        dt.Columns.Add("# Sales", typeof(int), computeString);
                        dt.Columns.Add("$ Sales", typeof(decimal), "[# Sales] * [Price]");


                        DataTable dtWeekly = new DataTable();
                        dtWeekly.Columns.Add("Location");

                        for (int i = 1; i <= 53; i++)
                        {
                            dtWeekly.Columns.Add(i.ToString());
                            System.Diagnostics.Debug.WriteLine(i);
                        }

                        var weekly = (from d in dt.AsEnumerable()
                                      group d by d.Field<string>("Location") into g
                                      select dtWeekly.LoadDataRow(
                                      new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["1"] != null ? x.Field<int>("1") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["2"] != null ? x.Field<int>("2") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["3"] != null ? x.Field<int>("3") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["4"] != null ? x.Field<int>("4") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["5"] != null ? x.Field<int>("5") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["6"] != null ? x.Field<int>("6") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["7"] != null ? x.Field<int>("7") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["8"] != null ? x.Field<int>("8") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["9"] != null ? x.Field<int>("9") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["10"] != null ? x.Field<int>("10") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["11"] != null ? x.Field<int>("11") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["12"] != null ? x.Field<int>("12") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["13"] != null ? x.Field<int>("13") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["14"] != null ? x.Field<int>("14") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["15"] != null ? x.Field<int>("15") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["16"] != null ? x.Field<int>("16") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["17"] != null ? x.Field<int>("17") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["18"] != null ? x.Field<int>("18") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["19"] != null ? x.Field<int>("19") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["20"] != null ? x.Field<int>("20") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["21"] != null ? x.Field<int>("21") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["22"] != null ? x.Field<int>("22") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["23"] != null ? x.Field<int>("23") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["24"] != null ? x.Field<int>("24") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["25"] != null ? x.Field<int>("25") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["26"] != null ? x.Field<int>("26") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["27"] != null ? x.Field<int>("27") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["28"] != null ? x.Field<int>("28") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["29"] != null ? x.Field<int>("29") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["30"] != null ? x.Field<int>("30") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["31"] != null ? x.Field<int>("31") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["32"] != null ? x.Field<int>("32") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["33"] != null ? x.Field<int>("33") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["34"] != null ? x.Field<int>("34") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["35"] != null ? x.Field<int>("35") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["36"] != null ? x.Field<int>("36") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["37"] != null ? x.Field<int>("37") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["38"] != null ? x.Field<int>("38") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["39"] != null ? x.Field<int>("39") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["40"] != null ? x.Field<int>("40") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["41"] != null ? x.Field<int>("41") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["42"] != null ? x.Field<int>("42") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["43"] != null ? x.Field<int>("43") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["44"] != null ? x.Field<int>("44") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["45"] != null ? x.Field<int>("45") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["46"] != null ? x.Field<int>("46") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["47"] != null ? x.Field<int>("47") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["48"] != null ? x.Field<int>("48") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["49"] != null ? x.Field<int>("49") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["50"] != null ? x.Field<int>("50") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["51"] != null ? x.Field<int>("51") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["52"] != null ? x.Field<int>("52") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["53"] != null ? x.Field<int>("53") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtWeekly);

                    }
                    else if (reportView.Equals("Monthly"))
                    {
                        showTotals = true;
                        dt = controller.GetMetricData(13, customerid, locationids, fromDate, toDate);

                        string computeString = "";

                        for (int i = 3; i < dt.Columns.Count; i++)
                        {
                            dt.Columns[i].ColumnName = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(Convert.ToInt32(dt.Columns[i].ColumnName));
                            computeString += "[" + dt.Columns[i].ColumnName + "] +";
                        }

                        computeString = computeString.Substring(0, computeString.Length - 1);

                        dt.Columns.Add("# Sales", typeof(int), computeString);
                        dt.Columns.Add("$ Sales", typeof(decimal), "[# Sales] * [Price]");

                        DataTable dtMonthly = new DataTable();
                        dtMonthly.Columns.Add("Location");
                        dtMonthly.Columns.Add("Jan");
                        dtMonthly.Columns.Add("Feb");
                        dtMonthly.Columns.Add("Mar");
                        dtMonthly.Columns.Add("Apr");
                        dtMonthly.Columns.Add("May");
                        dtMonthly.Columns.Add("Jun");
                        dtMonthly.Columns.Add("Jul");
                        dtMonthly.Columns.Add("Aug");
                        dtMonthly.Columns.Add("Sep");
                        dtMonthly.Columns.Add("Oct");
                        dtMonthly.Columns.Add("Nov");
                        dtMonthly.Columns.Add("Dec");

                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtMonthly.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["Jan"] != null ? x.Field<int>("Jan") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Feb"] != null ? x.Field<int>("Feb") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Mar"] != null ? x.Field<int>("Mar") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Apr"] != null ? x.Field<int>("Apr") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["May"] != null ? x.Field<int>("May") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Jun"] != null ? x.Field<int>("May") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Jul"] != null ? x.Field<int>("May") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Aug"] != null ? x.Field<int>("Aug") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Sep"] != null ? x.Field<int>("Sep") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Oct"] != null ? x.Field<int>("Oct") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Nov"] != null ? x.Field<int>("Nov") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["Dec"] != null ? x.Field<int>("Dec") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtMonthly);
                    }

                    //showCharts = true;
                    //chartData = PrepareChartObject(dt);


                }
                else if (reportType == "PaymentMethodByItemCount")
                {
                    DataTable dtPayment = new DataTable();
                    dtPayment.Columns.Add("Location");

                    if (reportView.Equals("Pay"))
                    {
                        dt = controller.GetMetricData(19, customerid, locationids, fromDate, toDate, "AND  Source Like '%Pay'");

                        dtPayment.Columns.Add("BillPay");
                        dtPayment.Columns.Add("CoinPay");
                        dtPayment.Columns.Add("CreditCardPay");
                        dtPayment.Columns.Add("MyAccountPay");
                        dtPayment.Columns.Add("MyPayrollPay");

                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["BillPay"] != null ? x.Field<int?>("BillPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinPay"] != null ? x.Field<int?>("CoinPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardPay"] != null ? x.Field<int?>("CreditCardPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["MyAccountPay"] != null ? x.Field<int?>("MyAccountPay") : 0;}),
                                         g.Sum(x => { return x.Table.Columns["MyPayrollPay"] != null ? x.Field<int?>("MyPayrollPay") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);

                    }
                    else if (reportView.Equals("Refill"))
                    {
                        //dt = controller.GetMetricData(5, customerid, locationids, fromDate, toDate, "AND Source Like '%Refill'");

                        dt = controller.GetMetricData(19, customerid, locationids, fromDate, toDate, "AND  Source Like '%Refill'");

                        dtPayment.Columns.Add("BillRefill");
                        dtPayment.Columns.Add("CoinRefill");
                        dtPayment.Columns.Add("CreditCardRefill");

                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["BillRefill"] != null ? x.Field<int?>("BillRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinRefill"] != null ? x.Field<int?>("CoinRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardRefill"] != null ? x.Field<int?>("CreditCardRefill") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);

                    }
                    else
                    {
                        dt = controller.GetMetricData(19, customerid, locationids, fromDate, toDate, "AND Source IN ('BillPay','BillRefill','CoinPay','CoinRefill','CreditCardPay','CreditCardRefill','MyAccountPay','MyPayrollPay')");

                        dtPayment.Columns.Add("BillPay");
                        dtPayment.Columns.Add("BillRefill");
                        dtPayment.Columns.Add("CoinPay");
                        dtPayment.Columns.Add("CoinRefill");
                        dtPayment.Columns.Add("CreditCardPay");
                        dtPayment.Columns.Add("CreditCardRefill");
                        dtPayment.Columns.Add("MyAccountPay");
                        dtPayment.Columns.Add("MyPayrollPay");


                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["BillPay"] != null ? x.Field<int?>("BillPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["BillRefill"] != null ? x.Field<int?>("BillRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinPay"] != null ? x.Field<int?>("CoinPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinRefill"] != null ? x.Field<int?>("CoinRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardPay"] != null ? x.Field<int?>("CreditCardPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardRefill"] != null ? x.Field<int?>("CreditCardRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["MyAccountPay"] != null ? x.Field<int?>("MyAccountPay") : 0;}),
                                          g.Sum(x => { return x.Table.Columns["MyPayrollPay"] != null ? x.Field<int?>("MyPayrollPay") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);


                    }
                }
                else if (reportType == "NoSales")
                {
                    dt = controller.GetMetricData(1, customerid, locationids, fromDate, toDate);
                }
                else if (reportType == "PaymentRouting")
                {
                    dt = controller.GetMetricData(31, customerid, locationids, fromDate, toDate);
                }
                else if (reportType == "PaymentMethodByAmount")
                {
                    DataTable dtPayment = new DataTable();
                    dtPayment.Columns.Add("Location");

                    if (reportView.Equals("Pay"))
                    {
                        dt = controller.GetMetricData(27, customerid, locationids, fromDate, toDate, "AND  Source Like '%Pay'");

                        dtPayment.Columns.Add("BillPay");
                        dtPayment.Columns.Add("CoinPay");
                        dtPayment.Columns.Add("CreditCardPay");
                        dtPayment.Columns.Add("MyAccountPay");
                        dtPayment.Columns.Add("MyPayrollPay");

                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["BillPay"] != null ? x.Field<decimal?>("BillPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinPay"] != null ? x.Field<decimal?>("CoinPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardPay"] != null ? x.Field<decimal?>("CreditCardPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["MyAccountPay"] != null ? x.Field<decimal?>("MyAccountPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["MyPayrollPay"] != null ? x.Field<decimal?>("MyPayrollPay") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);

                    }
                    else if (reportView.Equals("Refill"))
                    {
                        dt = controller.GetMetricData(27, customerid, locationids, fromDate, toDate, "AND  Source Like '%Refill'");

                        dtPayment.Columns.Add("BillRefill");
                        dtPayment.Columns.Add("CoinRefill");
                        dtPayment.Columns.Add("CreditCardRefill");

                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["BillRefill"] != null ? x.Field<decimal?>("BillRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinRefill"] != null ? x.Field<decimal?>("CoinRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardRefill"] != null ? x.Field<decimal?>("CreditCardRefill") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);

                    }
                    else
                    {
                        dt = controller.GetMetricData(27, customerid, locationids, fromDate, toDate, "AND Source IN ('BillPay','BillRefill','CoinPay','CoinRefill','CreditCardPay','CreditCardRefill','MyAccountPay','MyPayrollPay', 'Purchase Complete')");

                        dtPayment.Columns.Add("BillPay");
                        dtPayment.Columns.Add("BillRefill");
                        dtPayment.Columns.Add("CoinPay");
                        dtPayment.Columns.Add("CoinRefill");
                        dtPayment.Columns.Add("CreditCardPay");
                        dtPayment.Columns.Add("CreditCardRefill");
                        dtPayment.Columns.Add("MyAccountPay");
                        dtPayment.Columns.Add("MyPayrollPay");


                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["BillPay"] != null ? x.Field<decimal?>("BillPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["BillRefill"] != null ? x.Field<decimal?>("BillRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinPay"] != null ? x.Field<decimal?>("CoinPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinRefill"] != null ? x.Field<decimal?>("CoinRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardPay"] != null ? x.Field<decimal?>("CreditCardPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardRefill"] != null ? x.Field<decimal?>("CreditCardRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["MyAccountPay"] != null ? x.Field<decimal?>("MyAccountPay") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["MyPayrollPay"] != null ? x.Field<decimal?>("MyPayrollPay") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);


                    }

                }
                //======================New Payroll Report==================
                else if (reportType == "Payroll")
                {
                    DataTable dtPayment = new DataTable();
                    dtPayment.Columns.Add("Location");

                    if (reportView.Equals("Pay"))
                    {
                        dt = controller.GetMetricData(27, customerid, locationids, fromDate, toDate, "AND  Source Like '%Pay'");


                        dtPayment.Columns.Add("MyPayrollPay");

                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,                                       
                                        g.Sum(x => { return x.Table.Columns["MyPayrollPay"] != null ? x.Field<decimal?>("MyPayrollPay") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);

                    }
                    else if (reportView.Equals("Refill"))
                    {
                        dt = controller.GetMetricData(27, customerid, locationids, fromDate, toDate, "AND  Source Like '%Refill'");

                        dtPayment.Columns.Add("BillRefill");
                        dtPayment.Columns.Add("CoinRefill");
                        dtPayment.Columns.Add("CreditCardRefill");

                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                        g.Sum(x => { return x.Table.Columns["BillRefill"] != null ? x.Field<decimal?>("BillRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CoinRefill"] != null ? x.Field<decimal?>("CoinRefill") : 0;}),
                                        g.Sum(x => { return x.Table.Columns["CreditCardRefill"] != null ? x.Field<decimal?>("CreditCardRefill") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);

                    }
                    else
                    {
                        dt = controller.GetMetricData(27, customerid, locationids, fromDate, toDate, "AND Source IN ('MyPayrollPay')");


                        dtPayment.Columns.Add("MyPayrollPay");


                        var monthly = (from d in dt.AsEnumerable()
                                       group d by d.Field<string>("Location") into g
                                       select dtPayment.LoadDataRow(
                                       new object[] {
                                        g.Key,
                                       
                                        g.Sum(x => { return x.Table.Columns["MyPayrollPay"] != null ? x.Field<decimal?>("MyPayrollPay") : 0;}),
                                    }, true)).ToList();

                        chartData = PrepareChartObject(dtPayment);


                    }


                }
                //==========================================================
                else if (reportType == "ShoppingCartDetail")
                {
                    if (reportView == "Default")
                    {
                        dt = controller.GetMetricData(14, customerid, locationids, fromDate, toDate);
                    }
                    else
                    {
                        dt = controller.GetMetricData(24, customerid, locationids, fromDate, toDate);
                        groupColumn = 3;
                        dt.Columns[0].ColumnName = " ";
                    }
                }
             
            }




            if (dt.Columns.Count <= 0)
            {
                dt.Columns.Add("No Data");
            }

            var columns = from d in dt.Columns.Cast<DataColumn>()
                          select new
                          {
                              sTitle = d.ColumnName,
                              sClass = "center",
                          };

            List<string[]> rows = new List<string[]>();



            if (dt.Rows.Count == 0)
            {
                string[] NewRow = new string[columns.Count()];
                int i = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    if (i == 0)
                    {
                        NewRow[i] = "Location Selected";
                    }
                    else
                    {
                        NewRow[i] = "No Data";
                    }

                    i++;
                }

                rows.Add(NewRow);
            }
            else
            {
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

            }




            //return new JavaScriptSerializer().Serialize(new { columns, rows, showTotals, showSparkLines, data = chartData, color = "#3c8dbc" });
            return new JavaScriptSerializer().Serialize(new { columns, rows, showTotals, showSparkLines, data = chartData, groupColumn = groupColumn });


            //if (rows.Count == 0)
            //{
            //    string[] NewRow = new string[1];
            //    NewRow[1] = "0.00";

            //}

            //foreach (DataRow row in dt.Rows)
            //{

            //    string[] aRow = new string[columns.Count()];

            //    int i = 0;
            //    foreach (DataColumn col in dt.Columns)
            //    {
            //        aRow[i] = row[col].ToString();
            //        i++;
            //    }
            //    rows.Add(aRow);
            //}


        }

        private string GetPreviousServiceDate(string SelectedServiceDate, int LocationId, int CustomerId) {

            string PreviousServiceDate = "1/1/2000 12:00:00 AM";

            List<LocationServiceDTO> AllLocationServices = new List<LocationServiceDTO>();

            AllLocationServices = locationServiceRepo.GetLocationServices(LocationId, CustomerId).Where(x => x.comments == "Service Completed").ToList();

            foreach (var Service in AllLocationServices)
            {
                if (Service.created_date_time.ToString() != SelectedServiceDate)
                {
                    PreviousServiceDate = Service.created_date_time.ToString();
                }
                else {

                    return PreviousServiceDate;
                }
                
            }

            return PreviousServiceDate;
           
        }

        public List<object> PrepareChartObject(DataTable dt)
        {
            List<object> chartData = new List<object>();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    List<object> seriesData = new List<object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        seriesData.Add(new object[] { col.ColumnName, row[col] });
                    }

                    chartData.Add(new { label = row[0].ToString(), data = seriesData });
                }
            }

            return chartData;
        }

        public PartialViewResult GetCashAccountability(string pkid)
        {
            CashCounterRepository repo1 = new CashCounterRepository();
            CashReconciliationRepository repo2 = new CashReconciliationRepository();

            var data1 = repo1.GetCashAccountability(pkid);
            var data2 = repo2.GetCashAccountability(pkid);

            List<CashAccountabilityDTO> list = new List<CashAccountabilityDTO>();

            if (data1 != null)
                list.Add(data1);

            if (data2 != null)
                list.Add(data2);

            return PartialView("CashAccountabilityDetails", list);
        }


        public static DateTime FirstDateOfWeek(int year, int weekOfYear, System.Globalization.CultureInfo ci)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            DateTime firstWeekDay = jan1.AddDays(daysOffset);
            int firstWeek = ci.Calendar.GetWeekOfYear(jan1, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
            if (firstWeek <= 1 || firstWeek > 50)
            {
                weekOfYear -= 1;
            }
            return firstWeekDay.AddDays(weekOfYear * 7);
        }
    }

}


