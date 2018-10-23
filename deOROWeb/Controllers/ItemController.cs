using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    [deOROWeb.Security.deORORoleAuthorize(Roles = "Admin")]
    public class ItemController : MyBaseController
    {
        ItemRepository repo = new ItemRepository();

        public ActionResult Index(int id = 1)
        {
            this.ModelState.Clear();
            return View(repo.GetAll(id));
        }

        public ActionResult Details(int id = 0)
        {
            var item = repo.GetSingleById(id);

            if (item == null)
            {
                return HttpNotFound();
            }

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult IsBarcodeAvailable(string input_bar_code, string input_hidden_action)
        {
            if (input_hidden_action != "")
            {
                int itemId = Convert.ToInt32(input_hidden_action);
                var item = repo.GetSingleById(x => x.id == itemId);

                if (item.barcode == input_bar_code)
                {
                    return new JsonResult
                    {
                        Data = true,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    if (repo.GetSingleById(x => x.barcode == input_bar_code) != null)
                    {
                        return new JsonResult
                        {
                            Data = false,
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                    else
                    {
                        return new JsonResult
                        {
                            Data = true,
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                }

            }
            else
            {
                if (repo.GetSingleById(x => x.barcode == input_bar_code) != null)
                {
                    return new JsonResult
                    {
                        Data = false,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    return new JsonResult
                    {
                        Data = true,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
        }

        [HttpGet]
        public FileResult ExportItems()
        {
            DataTable dt = repo.GetAllToExport();

            string tmpFileName = DateTime.Now.Ticks + ".xlsx";
            string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\Templates\ExportItemsTemplate.xlsx";
            string tmpFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\Temp\" + tmpFileName;

            System.IO.File.Copy(templatePath, tmpFilePath);
            Helper.ExcelHelper.ExportDataTable(dt, tmpFilePath);

            FileStream stream = new FileStream(tmpFilePath, System.IO.FileMode.Open);
            string contentType = "application/vnd.ms-excel";

            return new FileStreamResult(stream, contentType) { FileDownloadName = tmpFileName };

        }

        [HttpPost]
        public string ImportItems(HttpPostedFileBase file)
        {
            var tempPath = AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\Upload\" + DateTime.Now.Ticks + ".xlsx";
            file.SaveAs(tempPath);
            string[] cols = { "id:System.Int32", "manufacturerid:System.Int32", "categoryid:System.Int32", 
                              "subcategoryid:System.Int32", "upc:System.String", "name:System.String", "barcode:System.String", 
                              "description:System.String", "count:System.Int32", "unitcost:System.Decimal", "avgshelflife:System.String", 
                              "pickorder:System.Int32", "is_taxable:System.Byte", "price:System.Decimal", "tax:System.Decimal", 
                              "price_tax_included:System.Decimal", "tax_percent:System.Decimal", "crv:System.Decimal", 
                              "has_barcode:System.Byte", "is_active:System.Byte" };

            return Helper.ExcelHelper.ImportItemsFromExcel(tempPath, cols);
        }

        [HttpPost]
        public ActionResult Edit(item item)
        {
            if (ModelState.IsValid)
            {
                if (Convert.ToBoolean(item.is_taxable))
                {
                    item.tax = ((item.price) + (item.crv == null ? 0 : item.crv.Value)) * (item.tax_percent.Value * 0.01m);
                    item.price_tax_included = Math.Round(((item.price) + (item.crv == null ? 0 : item.crv.Value)).Value + item.tax.Value, 2);
                }
                else
                    item.price_tax_included = Math.Round(((item.price) + (item.crv == null ? 0 : item.crv.Value)).Value, 2);

                repo.Edit(item);
                repo.Save();
            }

            return new EmptyResult();
        }

        [HttpPost]
        public void BatchEdit(int? manufactureid = null, int? categoryid = null, int? subcategoryid = null, byte? taxable = null,
                                      decimal? unitcost = null, int? count = null, decimal? price = null, decimal? taxpercent = null, decimal? crv = null,
                                      int[] itemids = null)
        {

            if (itemids != null)
            {
                repo.BatchEdit(manufactureid, categoryid, subcategoryid, taxable, unitcost,count, price, taxpercent, crv, itemids);
                repo.Save();
            }

        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var item = repo.GetSingleById(x => x.id == id);

                if (item != null)
                {
                    item.is_active = 0;
                    repo.Edit(item);
                    repo.Save();
                }

            }

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "id")] item item)
        {
            if (ModelState.IsValid)
            {
                if (Convert.ToBoolean(item.is_taxable))
                {
                    item.tax = ((item.price) + (item.crv == null ? 0 : item.crv.Value)) * (item.tax_percent == null ? 0 : item.tax_percent.Value * 0.01m);
                    item.price_tax_included = Math.Round(((item.price) + (item.crv == null ? 0 : item.crv.Value)).Value + item.tax.Value, 2);
                }
                else
                {
                    item.price_tax_included = Math.Round(((item.price) + (item.crv == null ? 0 : item.crv.Value)).Value, 2);
                }

                repo.Add(item);
                repo.Save();
            }

            return new EmptyResult();
        }

        public JsonResult GetItems(int active = 1)
        {
            var items = from c in repo.GetAll(active)
                        select new
                        {
                            id = c.id,
                            name = c.name
                        };

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemsByCategory(int categoryId, int subcategoryId)
        {
            var items = from c in repo.GetAll().Where(x => x.categoryid == categoryId && x.subcategoryid == subcategoryId)
                        select new
                        {
                            id = c.id,
                            name = c.name
                        };

            return Json(items, JsonRequestBehavior.AllowGet);
        }

    }
}