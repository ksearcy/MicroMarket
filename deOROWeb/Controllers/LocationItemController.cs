using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using deORODataAccess;


namespace deOROWeb.Controllers
{
    [deOROWeb.Security.deORORoleAuthorize(Roles = "Admin")]
    public class LocationItemController : MyBaseController
    {
        LocationItemRepository repo = new LocationItemRepository();

        public ActionResult Index(int id = 0)
        {
            ViewBag.locationid = id;
            //return View("Index", repo.GetAll(id));

            var items = repo.GetAll(id);

            //foreach (var item in items)
            //{
            //    item.chartdata = repo.GetSalesData(id, item.itemid, 15);
            //}

            return View("Index", items);
        }

        public JsonResult GetItems(int id = 0)
        {
            if (id == 0)
                return Json(null, JsonRequestBehavior.AllowGet);

            return Json(repo.GetAll(id).Select(x => new { id = x.itemid, name = x.itemname }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDiscountedItems(int locationid, int discountid)
        {
            if (locationid == 0)
                return Json(null, JsonRequestBehavior.AllowGet);

            return Json(repo.GetAllDiscountedItems(locationid, discountid).Select(x => new { id = x.itemid, name = x.itemname }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemsByIds(int locationid, int[] ids)
        {
            if (ids == null || ids.Count() <= 0)
                return Json(null, JsonRequestBehavior.AllowGet);

            StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\\Templates\PlanogramItemTemplate.html");
            string content = reader.ReadToEnd().Replace("\r\n", "");
            reader.Close();

            var items = repo.GetAll(locationid, ids);
            string itemname;

            foreach (var item in items)
            {
                if (item.itemname.Length > 13)
                    itemname = item.itemname.Substring(0, 10) + "..";
                else
                    itemname = item.itemname;

                item.html = String.Format(content, item.itemid, item.quantity, item.par, item.image, itemname, "", item.id,
                                         item.price_tax_included, item.itemname);
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSalesData(int locationid, int itemid, int days)
        {
            //if (locationid == null || itemid == null)
            //    return "";

            return Json(repo.GetSalesData(locationid, itemid, days), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAdjustedPar(int locationid, int itemid)
        {
            return Json(repo.GetAdjustedPar(locationid, itemid), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetParQuantity(int id)
        {
            //if (locationid == null || itemid == null)
            //    return "";

            return Json(repo.GetParQuantity(id), JsonRequestBehavior.AllowGet);
        }

        public EmptyResult UpdateParQuantity(int id, int par, int quantity)
        {
            repo.UpdateParQuantity(id, par, quantity);
            return new EmptyResult();
        }

        public EmptyResult UpdatePrice(int id, decimal price, decimal taxPercent, decimal crv)
        {
            repo.UpdatePrice(id, price, taxPercent, crv);
            return new EmptyResult();
        }

        public ActionResult AddRemoveItems(int locationid, int[] itemids)
        {
            repo.AddRemoveItems(locationid, itemids);
            return View();
        }

        public JsonResult UpdateRecommendedPar(int locationid, int[] itemids)
        {
            repo.UpdateRecommendedPar(locationid, itemids);
            return Json(locationid);
        }

        public EmptyResult UpdateDiscount(int locationid, int discountid, int[] itemids)
        {
            repo.UpdateDiscount(locationid, discountid, itemids);
            return new EmptyResult();
        }

        public string UpdateData(int? id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            location_item item = repo.GetSingleById(x => x.id == id);

            if (item != null) 
            {
                if (columnName.Equals(Resources.Strings.Tax))
                {
                    try
                    {
                        item.is_taxable = Convert.ToByte(value);
                    }
                    catch { }
                }
                else if (columnName.Equals(Resources.Strings.Price))
                {
                    try
                    {
                        item.price = Convert.ToDecimal(value);
                    }
                    catch
                    {
                        item.price = decimal.MinValue;
                    }
                }
                else if (columnName.Equals(Resources.Strings.Tax + " %"))
                {
                    try
                    {
                        item.tax_percent = Convert.ToDecimal(value);
                    }
                    catch
                    {
                        item.tax_percent = decimal.MinValue;
                    }
                }
                else if (columnName.Equals(Resources.Strings.CRV))
                {
                    try
                    {
                        item.crv = Convert.ToDecimal(value);
                    }
                    catch
                    {
                        item.crv = decimal.MinValue;
                    }
                }
                else if (columnName.Equals(Resources.Strings.Discount))
                {
                    try
                    {
                        item.discountid = Convert.ToInt32(value);
                    }
                    catch { }
                }
                else if (columnName.Equals(Resources.Strings.Par))
                {
                    try
                    {
                        item.par = Convert.ToInt32(value);
                    }
                    catch { }
                }
                else if (columnName.Equals(Resources.Strings.Quantity))
                {
                    try
                    {
                        item.quantity = Convert.ToInt32(value);
                    }
                    catch { }
                }
                else if (columnName.Equals("Depletion Level"))
                {
                    try
                    {
                        item.depletion_level = Convert.ToInt32(value);
                    }
                    catch { }
                }
                //else if (columnName.Equals("Subsidy"))
                //{
                //    try
                //    {
                //        item.subsidyid = Convert.ToInt32(value);
                //    }
                //    catch { }
                //}

                try
                {
                    item.tax = item.price * (item.tax_percent * 0.01m);
                }
                catch { }

                try
                {
                    item.price_tax_included = (item.price ?? 0) + (item.tax ?? 0) + (item.crv ?? 0);
                }
                catch { }

                repo.Edit(item);
                repo.Save();

                return new JavaScriptSerializer().Serialize(new { item });
            }
            else
            {
                return "";
            }
        }

        [HttpGet]
        public FileResult ExportLocationItems()
        {
            var location_id = Convert.ToInt32(Request.QueryString["location_id"]);

            //DataTable dt = repo.GetAllLocationItemToExport();
            DataTable dt = repo.GetLocationItemToExportById(location_id);

            //string tmpFileName = "Location Items_"+  DateTime.Now.ToShortDateString().Replace('/','_') + ".xlsx";
            string tmpFileName = "Location_#" + location_id.ToString() + "_Items_" + DateTime.Now.Ticks + ".xlsx";
            string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\Templates\ExportLocationItemsTemplate.xlsx";
            string tmpFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\Temp\" + tmpFileName;

            if (System.IO.File.Exists(tmpFilePath))
            {
                System.IO.File.Delete(tmpFilePath);
            }


            System.IO.File.Copy(templatePath, tmpFilePath);
            Helper.ExcelHelper.ExportDataTable(dt, tmpFilePath);

            FileStream stream = new FileStream(tmpFilePath, System.IO.FileMode.Open);
            string contentType = "application/vnd.ms-excel";

            return new FileStreamResult(stream, contentType) { FileDownloadName = tmpFileName };
        }

        //[HttpGet]
        //public FileResult ExportLocationItems()
        //{
        //    DataTable dt = repo.GetAllLocationItemToExport();

        //    string tmpFileName = DateTime.Now.Ticks + ".xlsx";
        //    string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\Templates\ExportLocationItemsTemplate.xlsx";
        //    string tmpFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\Temp\" + tmpFileName;

        //    System.IO.File.Copy(templatePath, tmpFilePath);
        //    Helper.ExcelHelper.ExportDataTable(dt, tmpFilePath);

        //    FileStream stream = new FileStream(tmpFilePath, System.IO.FileMode.Open);
        //    string contentType = "application/vnd.ms-excel";

        //    return new FileStreamResult(stream, contentType) { FileDownloadName = tmpFileName };
        //}

        [HttpPost]
        public string ImportLocationItems(HttpPostedFileBase file)
        {
            var tempPath = AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\Upload\" + DateTime.Now.Ticks + ".xlsx";
            file.SaveAs(tempPath);
            string[] cols = { "id:System.Int32", "customerid:System.String", "locationid:System.String", 
                              "itemid:System.Int32", "itemname:System.String", "discountid:System.Int32", "is_taxable:System.Byte", 
                              "price:System.Decimal", "tax:System.Decimal", "price_tax_included:System.Decimal", "tax_percent:System.Decimal", 
                              "crv:System.Decimal", "par:System.Int32", "quantity:System.Int32", "depletion_level:System.Decimal", 
                              "created_date_time:System.Date", "is_active:System.Byte"};

            return Helper.ExcelHelper.ImportLocationItemsFromExcel(tempPath, cols);
        }

        //public ActionResult GetLocationItems(int id = 0)
        //{
        //    return Index();
        //    ///return View("Index", repo.GetAll(id));
        //    //return Redirect(
        //    //RedirectToAction("Index", repo.GetAll(id));

        //    //if (id == 0)
        //    //    return Json(null, JsonRequestBehavior.AllowGet);

        //    //DataTable dt = repo.GetAll(id).ToDataTable();

        //    //var columns = from d in dt.Columns.Cast<DataColumn>()
        //    //              select new
        //    //              {
        //    //                  sTitle = d.ColumnName,
        //    //                  sClass = "center"
        //    //              };

        //    //List<string[]> rows = new List<string[]>();


        //    //foreach (DataRow row in dt.Rows)
        //    //{
        //    //    string[] aRow = new string[columns.Count()];

        //    //    int i = 0;
        //    //    foreach (DataColumn col in dt.Columns)
        //    //    {
        //    //        aRow[i] = row[col].ToString();
        //    //        i++;
        //    //    }
        //    //    rows.Add(aRow);
        //    //}

        //    //return new JavaScriptSerializer().Serialize(new { columns, rows });
        //}
    }
}
