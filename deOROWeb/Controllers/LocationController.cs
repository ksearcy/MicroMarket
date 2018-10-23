using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess.DTO;
using deORODataAccess;
using System.ComponentModel;
using System.Globalization;
using System.Web.Services;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace deOROWeb.Controllers
{
    [deOROWeb.Security.deORORoleAuthorize(Roles = "Admin")]
    public class LocationController :  MyBaseController
    {
        LocationRepository repo = new LocationRepository();
        LocationServiceRepository repolocationservice = new LocationServiceRepository();
        CustomerRepository repocust = new CustomerRepository();
        UserRepository repoUser = new UserRepository();

        public ActionResult Index()
        {
            ViewBag.Customers = repocust.GetAll().ToList();
            ViewBag.Users = repoUser.FindBy(x => x.is_staff == 1);

            return View(repo.GetAll());
        }

        public ActionResult Details(int id = 0)
        {
            var location = repo.GetSingleById(id);

            if (location == null)
            {
                return HttpNotFound();
            }          

            return Json(location, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Edit(location location_Data, user user_Data)
        {

            string NewUserpkID = Guid.NewGuid().ToString();

            if (ModelState.IsValid)
            {

                if (user_Data != null)
                {
                    location_Data.driverid = NewUserpkID;
                    repo.Edit(location_Data);
                    repo.Save();
                    user_Data.pkid = NewUserpkID;
                    user_Data.salt = GetRandomSalt();
                    user_Data.password = GetPasswordHash(user_Data.password, user_Data.salt);
                    repoUser.Add(user_Data);
                    repoUser.Save();
                }
                else
                {
                    repo.Edit(location_Data);
                    repo.Save();
                }

                return RedirectToAction("Index");
            }

            return View("Index");
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "id")] location location_Data, user user_Data)
        {
            string NewUserpkID = Guid.NewGuid().ToString();

            if (ModelState.IsValid)
            {
                if (user_Data != null)
                {
                    location_Data.driverid = NewUserpkID;
                    repo.Edit(location_Data);
                    repo.Save();
                    user_Data.pkid = NewUserpkID;
                    user_Data.salt = GetRandomSalt();
                    user_Data.password = GetPasswordHash(user_Data.password, user_Data.salt);
                    repoUser.Add(user_Data);
                    repoUser.Save();
                }
                else
                {
                    repo.Edit(location_Data);
                    repo.Save();
                }

                return RedirectToAction("Index");
            }

            return View("Index");
        }

        //[HttpPost]
        //public ActionResult Edit(location location)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        repo.Edit(location);
        //        repo.Save();
        //        return RedirectToAction("Index");
        //    }

        //    return View("Index");
        //}


        //[HttpPost]
        //public ActionResult Create([Bind(Exclude = "id")] location location)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        repo.Add(location);
        //        repo.Save();
        //        return RedirectToAction("Index");
        //    }

        //    return View("Index");
        //}

        [HttpPost]
        public JsonResult IsCodeAvailable(string input_location_code, string input_hidden_action)
        {
            if (input_hidden_action != "")
            {
                return new JsonResult
                {
                    Data = true,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            var location = repo.GetSingleById(x => x.code == input_location_code);

            if (location != null)
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
        public JsonResult IsNameAvailable(string input_location_name, string input_hidden_action)
        {
            if (input_hidden_action != "")
            {
                return new JsonResult
                {
                    Data = true,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            var location = repo.GetSingleById(x => x.name == input_location_name);

            if (location != null)
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
        public JsonResult IsUsernameAvailable(string input_user_name, string input_hidden_action)
        {
            if (input_hidden_action != "")
            {
                return new JsonResult
                {
                    Data = true,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            var location = repo.GetSingleById(x => x.username == input_user_name);

            if (location != null)
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
        public JsonResult IsEmailAvailable(string input_email_address, string input_hidden_action)
        {
            var location = repo.GetSingleById(x => x.email_address == input_email_address);

            if (location != null)
            {
                if (input_hidden_action != "")
                {
                    if (location.id == Convert.ToInt32(input_hidden_action))
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

        //=============================Vrify if the username is already on the location==============

        [HttpPost]
        public JsonResult IsLoginUsernameAvailable(string input_user_username)
        {
            //if (input_hidden_action != "")
            //{
            //    return new JsonResult
            //    {
            //        Data = true,
            //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
            //    };
            //}

            var user = repoUser.GetSingleById(x => x.username == input_user_username);

            if (user != null)
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
         
        public JsonResult GetLocations(int id = 0)
        {
            var locations = from c in repo.GetAll(id)
                            select new
                            {
                                id = c.id,
                                name = c.name
                            };

            return Json(locations, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetLocationServices(string locationid, string customerid )
        [WebMethod]
        public JsonResult GetLocationServices(int locationid = 0, int customerid = 0)
        {
            var locationservices = from c in repolocationservice.GetLocationServices(locationid, customerid).Where(x => x.comments == "Service Completed")
                            select new
                            {
                                id = c.id,
                                servicedate = c.created_date_time.ToString()
                            };

            return Json(locationservices, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCameraFeedPath(int id = 0)
        {
            var location = repo.GetSingleById(id);
            string path = "Location Null";

            if (location != null)
            {
                path = Server.MapPath("~/CameraFeed/") + location.camera_feed_path;
            }

            return Json(path, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCameras(int id = 0)
        {
            var location = repo.GetSingleById(id);
            dynamic directories = new { };

            if (location != null && location.camera_feed_path != null)
            {
                //directories = from c in System.IO.Directory.EnumerateDirectories(location.camera_feed_path)
                //directories = from c in System.IO.Directory.EnumerateDirectories(Server.MapPath("~/") + "\\CameraFeed\\" + location.camera_feed_path)

                directories = from c in System.IO.Directory.EnumerateDirectories(Server.MapPath("~/CameraFeed/") + location.camera_feed_path)

                              select new
                              {
                                  id = c,
                                  name = System.IO.Path.GetFileName(c)
                              };
            }

            return Json(directories, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCameraFeed(int locationid, string camera, string feedPath, string cameraDate, string fromTime, string toTime)
        {
            List<string> playList = new List<string>();
            var l = repo.GetSingleById(locationid);

            try
            {
                DateTime fromDateTime = DateTime.Parse(cameraDate + " " + fromTime);
                DateTime toDateTime = DateTime.Parse(cameraDate + " " + toTime);

                string searchPattern = string.Format("*_{0}-{1}-{2}*", fromDateTime.Year, fromDateTime.Month.ToString().PadLeft(2, '0'),
                                       fromDateTime.Day.ToString().PadLeft(2, '0'));

                var files = System.IO.Directory.EnumerateFiles(feedPath, searchPattern);

                string format = "yyyy-MM-dd HH-mm-ss";
                CultureInfo provider = CultureInfo.InvariantCulture;

                foreach (string s in files)
                {
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(s);
                    fileName = fileName.Substring(2).Replace("_", " ");
                    DateTime dt = DateTime.ParseExact(fileName, format, provider);

                    if (dt >= fromDateTime && dt <= toDateTime)
                        playList.Add(l.camera_feed_path + "/" + camera + "/" + System.IO.Path.GetFileName(s));
                }
            }
            catch { }

            return Json(playList, JsonRequestBehavior.AllowGet);
        }

        //==================Methods to encript Password====================

        public static string GetPasswordHash(string password, string salt)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(password + salt);
            SHA256 s = new SHA256CryptoServiceProvider();

            return BitConverter.ToString(s.ComputeHash(inputBytes)).Replace("-", "").ToLower();
        }

        public static string GetRandomSalt(int length = 25)
        {
            RNGCryptoServiceProvider rncCsp = new RNGCryptoServiceProvider();
            byte[] salt = new byte[length];
            rncCsp.GetBytes(salt);

            return BitConverter.ToString(salt).Replace("-", "");
        }

        //======================================================================

    }
}