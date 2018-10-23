using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using deORODataAccess;
using System.Security.Cryptography;
using System.Text;
using System.Data.Objects;

namespace deOROWeb.Controllers
{
    [deOROWeb.Security.deORORoleAuthorize(Roles = "Admin")]
    public class UserController : MyBaseController
    {
        UserRepository repo = new UserRepository();
        LocationRepository repoLocation = new LocationRepository();
        AccountBalanceHistoryRepository AccountBalanceHistoryRepository = new AccountBalanceHistoryRepository();


        public ActionResult Index(int customerid = 0, int locationid = 0, byte usersshared = 0)
        {
            return View(repo.GetAll(customerid, locationid, usersshared));
        }

        public JsonResult GetUsers(int customerid = 0, int locationid = 0, byte usersshared = 0)
        {
            var users = from c in repo.GetAll(customerid, locationid, usersshared)
                        select new
                        {
                            id = c.pkid,
                            name = c.username,
                            barcode = c.barcode

                        };

            return Json(users, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetLastUser()
        {

            List<user> users = new List<user>{
                   new user{ id = repo.GetLastUser().id}                   
                   };

            return Json(users, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetUserDetails(int id = 0)
        {
            var user = repo.GetSingleById(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return Json(user, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult Edit(user user_Data)
        {
            //string NewUserpkID = Guid.NewGuid().ToString();

            if (ModelState.IsValid)
            {

                user User_Update_Data = repo.GetSingleById(user_Data.id);

                User_Update_Data.first_name = user_Data.first_name;
                User_Update_Data.last_name = user_Data.last_name;
                User_Update_Data.barcode = user_Data.barcode;
                User_Update_Data.username = user_Data.username;
                User_Update_Data.email = user_Data.email;
                User_Update_Data.is_staff = user_Data.is_staff;
                User_Update_Data.is_active = user_Data.is_active;
                User_Update_Data.is_superuser = user_Data.is_superuser;
                User_Update_Data.last_updated_on = user_Data.last_updated_on;
                User_Update_Data.is_lockedout = user_Data.is_lockedout;

                if (User_Update_Data.sync_vector != null)
                {
                    User_Update_Data.sync_vector = User_Update_Data.sync_vector + 1;
                }
                else
                {
                    User_Update_Data.sync_vector = 1;
                }


                if (User_Update_Data.account_balance != user_Data.account_balance)
                {
                    User_Update_Data.lastaccountbalancechangedamount = User_Update_Data.account_balance;
                    User_Update_Data.account_balance = user_Data.account_balance;
                    User_Update_Data.lastaccountbalancechangeddate = user_Data.last_updated_on;
                    User_Update_Data.lastaccountbalancechangeddescription = "Web App Change";

                    accountbalancehistory ChangeBalanceRegistration = new accountbalancehistory();


                    decimal? AmountDifference = 0;
                    string ChangeDescription;

                    if (User_Update_Data.lastaccountbalancechangedamount != null && User_Update_Data.lastaccountbalancechangedamount < User_Update_Data.account_balance)
                    {
                        AmountDifference = User_Update_Data.account_balance - User_Update_Data.lastaccountbalancechangedamount;
                        ChangeDescription = "Account Balance Web Page Increase";
                    }
                    else if (User_Update_Data.lastaccountbalancechangedamount != null && User_Update_Data.lastaccountbalancechangedamount > User_Update_Data.account_balance)
                    {
                        AmountDifference = User_Update_Data.lastaccountbalancechangedamount - User_Update_Data.account_balance;
                        AmountDifference = AmountDifference * -1;
                        ChangeDescription = "Account Balance Web Page Decrease";
                    }
                    else
                    {
                        AmountDifference = User_Update_Data.account_balance;
                        ChangeDescription = "Account Balance Web Page Increase";
                    }

                    ChangeBalanceRegistration.account_balance = user_Data.account_balance;
                    ChangeBalanceRegistration.amount = AmountDifference;
                    ChangeBalanceRegistration.description = ChangeDescription;
                    ChangeBalanceRegistration.created_date_time = User_Update_Data.last_updated_on;
                    ChangeBalanceRegistration.locationid = User_Update_Data.locationid;
                    ChangeBalanceRegistration.userpkid = User_Update_Data.pkid;
                    ChangeBalanceRegistration.customerid = User_Update_Data.customerid;
                    ChangeBalanceRegistration.pkid = Guid.NewGuid().ToString();

                    AccountBalanceHistoryRepository.Add(ChangeBalanceRegistration);
                    AccountBalanceHistoryRepository.Save();
                }


                if (user_Data.password != "" && user_Data.password != null)
                {
                    User_Update_Data.salt = GetRandomSalt();
                    User_Update_Data.password = GetPasswordHash(user_Data.password, User_Update_Data.salt);
                }


                if (user_Data.locationid != 0)
                {
                    var User_locationid = Convert.ToInt32(user_Data.locationid);

                    string User_LocationUserSharedInfo = repoLocation.GetSingleById(User_locationid).users_shared.ToString();

                    if (User_LocationUserSharedInfo == "1")
                    {
                        User_Update_Data.locationid = 0;
                    }
                }



                repo.Edit(User_Update_Data);

                repo.Save();

                return RedirectToAction("Index");
            }

            return View("Index");
        }


        [HttpPost]
        public ActionResult Create([Bind(Exclude = "id")] user user_Data)
        {
            //string NewUserpkID = Guid.NewGuid().ToString();

            if (ModelState.IsValid)
            {
                user_Data.pkid = Guid.NewGuid().ToString();
                user_Data.salt = GetRandomSalt();
                user_Data.password = GetPasswordHash(user_Data.password, user_Data.salt);
                user_Data.is_lockedout = 0;
                user_Data.sync_vector = 0;                

                if (user_Data.locationid != 0)
                {
                    var User_locationid = Convert.ToInt32(user_Data.locationid);

                    string User_LocationUserSharedInfo = repoLocation.GetSingleById(User_locationid).users_shared.ToString();

                    if (User_LocationUserSharedInfo == "1")
                    {
                        user_Data.locationid = 0;
                    }
                }

                repo.Add(user_Data);
                repo.Save();

                return RedirectToAction("Index");
            }

            return View("Index");
        }


        [HttpPost]
        public JsonResult IsLoginUsernameAvailable(string input_user_username, string input_hidden_action)
        {
            if (input_hidden_action != "")
            {
                var userid = 0;

                userid = Convert.ToInt32(input_hidden_action);

                var Actual_User_Details = repo.GetSingleById(userid);

                if (Actual_User_Details.username == input_user_username)
                {
                    return new JsonResult
                    {
                        Data = true,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };

                }
                else
                {

                    var user = repo.GetSingleById(x => x.username == input_user_username);

                    if (user != null)
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


                var user = repo.GetSingleById(x => x.username == input_user_username);

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

        }


        [HttpPost]
        public JsonResult IsBarcodeAvailable(string input_user_barcode, string input_hidden_action)
        {
            if (input_hidden_action != "")
            {
                var userid = 0;

                userid = Convert.ToInt32(input_hidden_action);

                var Actual_User_Details = repo.GetSingleById(userid);

                if (Actual_User_Details.barcode == input_user_barcode)
                {
                    return new JsonResult
                    {
                        Data = true,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };

                }
                else
                {

                    var user = repo.GetSingleById(x => x.barcode == input_user_barcode);

                    if (user != null)
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


                var user = repo.GetSingleById(x => x.barcode == input_user_barcode);

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
