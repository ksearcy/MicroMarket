using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;

namespace deOROWeb.Controllers
{
    [deOROWeb.Security.deORORoleAuthorize(Roles = "Admin")]
    public class CustomerController : MyBaseController
    {
        CustomerRepository repo = new CustomerRepository();

        public ActionResult Index()
        {
            return View(repo.GetAll().ToList());
        }

        public ActionResult Details(int id = 0)
        {
            customer customer = repo.GetSingleById(x => x.id == id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return Json(customer, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(customer customer)
        {
            if (ModelState.IsValid)
            {
                repo.Edit(customer);
                repo.Save();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "id")] customer customer)
        {
            if (ModelState.IsValid)
            {
                repo.Add(customer);
                repo.Save();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        public JsonResult GetCustomers()
        {
            var customers = from c in repo.GetAll()
                            select new
                            {
                                id = c.id,
                                name = c.name
                            };

            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult IsNameAvailable(string input_customer_name, string input_hidden_action)
        {

            if (input_hidden_action != "")
            {
                return new JsonResult
                {
                    Data = true,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            var customer = repo.GetSingleById(x => x.name == input_customer_name);

            if (customer != null)
            {
                return new JsonResult
                {
                    Data = false,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
                return new JsonResult
                {
                    Data = true,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

        }

        [HttpPost]
        public JsonResult IsCodeAvailable(string input_customer_code, string input_hidden_action)
        {

            if (input_hidden_action != "")
            {
                return new JsonResult
                {
                    Data = true,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            var customer = repo.GetSingleById(x => x.code == input_customer_code);

            if (customer != null)
            {
                return new JsonResult
                {
                    Data = false,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
                return new JsonResult
                {
                    Data = true,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

        }

        [HttpPost]
        [ValidateInput(false)]
        public EmptyResult SaveRemote(string content)
        {

            return new EmptyResult();
        }

        [HttpPost]
        public JsonResult IsEmailAvailable(string input_email_address, string input_hidden_action)
        {
            var customer = repo.GetSingleById(x => x.email_address == input_email_address);

            if (customer != null)
            {
                if (input_hidden_action != "")
                {
                    if (customer.id == Convert.ToInt32(input_hidden_action))
                    {
                        return new JsonResult
                        {
                            Data = true,
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                    else
                    {
                        return new JsonResult
                        {
                            Data = false,
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                }
                else
                {
                    return new JsonResult
                    {
                        Data = false,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
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
}
