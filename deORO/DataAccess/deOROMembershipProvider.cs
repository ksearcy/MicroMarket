using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using deORO.Models;
using DPUruNet;


namespace deORO.DataAccess
{
    class deOROMembershipProvider : MembershipProvider
    {
        private string applicationName;
        deOROEntities entities = new deOROEntities();

        public override string ApplicationName
        {
            get
            {
                return applicationName;
            }
            set
            {
                applicationName = "deORO";
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user = entities.users.SingleOrDefault(x => x.username == username);

            if (user != null)
            {
                user.password = newPassword;
                return Convert.ToBoolean(entities.SaveChanges());
            }
            else
            {
                return false;
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }


        public MembershipUser CreateUser(string username, string password, string email, string passwordQuestion,
                                         string passwordAnswer, bool isApproved, object providerUserKey, decimal accountBalance, string barcode,
                                         out MembershipCreateStatus status)
        {
            MembershipUser user = GetUser(username, true);

            if (user != null)
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;

            }
            else if (GetUserNameByEmail(email) != null)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }
            else if (GetUserNameByBarcode(barcode) != null)
            {
                status = MembershipCreateStatus.UserRejected;
                return null;
            }
            else if (user == null)
            {
                user u = new user();
                u.username = username;
                u.password = password;
                u.email = email;
                u.created_date_time = DateTime.Now;
                u.is_approved = 1;
                u.is_active = 1;
                u.is_lockedout = 0;
                u.pkid = providerUserKey.ToString();
                u.account_balance = accountBalance;
                u.is_superuser = 0;
                u.pkid = Guid.NewGuid().ToString();
                u.barcode = barcode;

                entities.users.Add(u);
                entities.SaveChanges();

                status = MembershipCreateStatus.Success;
                return GetUser(username, true);
            }

            status = MembershipCreateStatus.ProviderError;
            return null;
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion,
                                                  string passwordAnswer, bool isApproved, object providerUserKey,
                                                  out MembershipCreateStatus status)
        {
            MembershipUser user = GetUser(username, true);

            if (user != null)
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;

            }
            else if (GetUserNameByEmail(email) != null)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            else if (user == null)
            {
                user u = new user();
                u.username = username;
                u.password = password;
                u.email = email;
                u.created_date_time = DateTime.Now;
                u.is_approved = 1;
                u.is_active = 1;
                u.is_lockedout = 0;
                u.pkid = providerUserKey.ToString();
                u.account_balance = 0;
                u.is_superuser = 0;
                u.pkid = Guid.NewGuid().ToString();

                entities.users.Add(u);
                entities.SaveChanges();

                status = MembershipCreateStatus.Success;
                return GetUser(username, true);
            }

            status = MembershipCreateStatus.ProviderError;
            return null;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            var user = entities.users.AsNoTracking().SingleOrDefault(x => x.username == username);

            if (user != null)
                return user.password;
            else
                return null;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline = true)
        {
            var user = entities.users.AsNoTracking().SingleOrDefault(x => x.username == username);

            if (user != null)
            {
                deoOROMembershipUser deoUser = new deoOROMembershipUser(user.id, "deOROMembershipProvider", user.username, user.pkid, user.email,
                                                                        Convert.ToBoolean(user.is_approved.Value),
                                                                        Convert.ToBoolean(user.is_lockedout.Value),                                     user.created_date_time.GetValueOrDefault(),
                                                                        user.last_login.GetValueOrDefault(),
                                                                        user.lastpasswordchangedate.GetValueOrDefault(),
                                                                        user.lastlockedoutdate.GetValueOrDefault(),
                                                                        user.account_balance.Value, Convert.ToBoolean(user.is_superuser.Value), user.barcode,user.password);
                return deoUser;
            }

            return null;

        }

        public MembershipUser GetUserByBarcode(string barcode)
        {
            var users = entities.users.AsNoTracking().Where(x => x.barcode == barcode).ToList();

            if (users.Count() > 0)
            {
                var user = users[0];

                if (user != null)
                {
                    deoOROMembershipUser deoUser = new deoOROMembershipUser(user.id, "deOROMembershipProvider", user.username, user.pkid, user.email,
                                                                            Convert.ToBoolean(user.is_approved.Value),
                                                                            Convert.ToBoolean(user.is_lockedout.Value), 
                                                                            user.created_date_time.GetValueOrDefault(),
                                                                            user.last_login.GetValueOrDefault(),
                                                                            user.lastpasswordchangedate.GetValueOrDefault(),
                                                                            user.lastlockedoutdate.GetValueOrDefault(),
                                                                            user.account_balance.Value, 
                                                                            Convert.ToBoolean(user.is_superuser.Value), user.barcode, user.password);
                    return deoUser;

                }
                else
                    return null;

            }

            return null;

        }

        public void LogSuccessfulLogin(string userName, Helpers.Enum.AuthenticationMode authenticationMode)
        {
            var usr = entities.users.SingleOrDefault(x => x.username == userName);

            if (usr != null)
            {
                usr.lastlogindate = DateTime.Now;
                usr.failedpasswordattemptcount = 0;
                usr.lastlogintype = authenticationMode.ToString();
                entities.SaveChanges();
            }
        }

        public void LogFailedLogin(string userName, Helpers.Enum.AuthenticationMode authenticationMode)
        {
            var usr = entities.users.SingleOrDefault(x => x.username == userName);

            if (usr != null)
            {
                usr.lastlogintype = authenticationMode.ToString();
                usr.failedpasswordattemptcount = usr.failedpasswordattemptcount + 1;
                entities.SaveChanges();
            }
        }


        public bool UpdateFingerprints(MembershipUser user, Dictionary<int, DataResult<Fmd>> fingerPrints)
        {
            try
            {
                var usr = entities.users.SingleOrDefault(x => x.username == user.UserName);

                if (usr != null)
                {
                    usr.lastfmdchangeddate = DateTime.Now;
                    usr.enrolled_fmd1 = null;
                    usr.enrolled_fmd2 = null;
                    usr.enrolled_fmd3 = null;
                    usr.enrolled_fmd4 = null;

                    int i = 1;
                    foreach (int fingerIdx in fingerPrints.Keys)
                    {
                        if (fingerPrints[fingerIdx] != null)
                        {
                            switch (i)
                            {
                                case 1:
                                    {
                                        try
                                        {
                                            usr.enrolled_fmd1 = Fmd.SerializeXml(fingerPrints[fingerIdx].Data);
                                            usr.finger_id1 = fingerIdx;
                                            break;
                                        }
                                        catch
                                        {
                                            return false;
                                        }
                                    }
                                case 2:
                                    {
                                        try
                                        {
                                            usr.enrolled_fmd2 = Fmd.SerializeXml(fingerPrints[fingerIdx].Data);
                                            usr.finger_id2 = fingerIdx;
                                            break;
                                        }
                                        catch
                                        {
                                            return false;
                                        }
                                    }
                                case 3:
                                    {
                                        try
                                        {
                                            usr.enrolled_fmd3 = Fmd.SerializeXml(fingerPrints[fingerIdx].Data);
                                            usr.finger_id3 = fingerIdx;
                                            break;
                                        }
                                        catch
                                        {
                                            return false;
                                        }
                                    }
                                case 4:
                                    {
                                        try
                                        {
                                            usr.enrolled_fmd4 = Fmd.SerializeXml(fingerPrints[fingerIdx].Data);
                                            usr.finger_id4 = fingerIdx;
                                            break;
                                        }
                                        catch
                                        {
                                            return false;
                                        }
                                    }
                            }
                        }

                        i++;
                    }
                }

                entities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            var user = entities.users.SingleOrDefault(x => x.email == email);

            if (user != null)
            {
                return user.username;
            }

            return null;
        }

        public string GetUserNameByBarcode(string barcode)
        {
            var user = entities.users.SingleOrDefault(x => x.barcode == barcode);

            if (user != null)
            {
                return user.username;
            }

            return null;
        }

        public MembershipUser GetUserByEmail(string email)
        {
            var user = entities.users.SingleOrDefault(x => x.email == email);

            if (user != null)
            {
                deoOROMembershipUser deoUser = new deoOROMembershipUser(user.id, "deOROMembershipProvider", user.username, user.pkid, user.email,
                                                                           Convert.ToBoolean(user.is_approved.Value),
                                                                           Convert.ToBoolean(user.is_lockedout.Value),
                                                                           user.created_date_time.GetValueOrDefault(),
                                                                           user.last_login.GetValueOrDefault(),
                                                                           user.lastpasswordchangedate.GetValueOrDefault(),
                                                                           user.lastlockedoutdate.GetValueOrDefault(),
                                                                           user.account_balance.Value,
                                                                           Convert.ToBoolean(user.is_superuser.Value), user.barcode, user.password);
                return deoUser;
            }
            else
            {
                return null;
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        //public bool DeductUserBalance(string userName, decimal amount)
        //{
        //    var user = entities.users.SingleOrDefault(x => x.username == userName);
        //    if (user != null)
        //    {
        //        user.account_balance -= amount;
        //        user.lastaccountbalancechangedamount = amount;
        //        user.lastaccountbalancechangeddate = DateTime.Now;
        //        user.lastaccountbalancechangeddescription = "Purchase";
        //        entities.SaveChanges();

        //        return true;
        //    }
        //    else
        //        return false;
        //}

        //public bool AddToUserBalance(string userName, decimal amount, string description)
        //{
        //    var user = entities.users.SingleOrDefault(x => x.username == userName);
        //    if (user != null)
        //    {
        //        user.account_balance += amount;
        //        user.lastaccountbalancechangedamount = amount;
        //        user.lastaccountbalancechangeddate = DateTime.Now;
        //        user.lastaccountbalancechangeddescription = description;
        //        entities.SaveChanges();

        //        return true;
        //    }
        //    else
        //        return false;
        //}

        public bool UpdateUserBalance(string userName, decimal amount, string description)
        {
            var user = entities.users.SingleOrDefault(x => x.username == userName);
            if (user != null)
            {
                user.account_balance += amount;
                user.lastaccountbalancechangedamount = amount;
                user.lastaccountbalancechangeddate = DateTime.Now;
                user.lastaccountbalancechangeddescription = description;
                entities.SaveChanges();

                return true;
            }
            else
                return false;
        }

        public bool UpdateBarcode(string userName, string barcode)
        {
            var user = entities.users.SingleOrDefault(x => x.username == userName);

            if (user != null)
            {
                user.barcode = barcode;
                entities.SaveChanges();

                return true;
            }

            return false;
        }

        public void UpdateEmail(string userName, string emailAddress, out MembershipCreateStatus status)
        {
            if (GetUserNameByEmail(emailAddress) != null)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return;
            }

            var user = entities.users.SingleOrDefault(x => x.username == userName);

            if (user != null)
            {
                user.email = emailAddress;
                entities.SaveChanges();

                status = MembershipCreateStatus.Success;
                return;
            }

            status = MembershipCreateStatus.ProviderError;
        }

        public override bool ValidateUser(string username, string password)
        {
            var usr = (entities.users.AsNoTracking().SingleOrDefault(x => x.username == username && x.is_active == 1 && x.is_approved == 1 && x.is_lockedout == 0));

            if (usr != null)
            {
                if (usr.password == password)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool ValidateUser(string barcode)
        {
            var usr = entities.users.SingleOrDefault(x => x.barcode == barcode);

            if (usr != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object[] GetAllUsersForFmdIdentification()
        {
            object[] ret = new object[6];
            List<string> retPKID = new List<string>(); // 0
            List<string> retUserName = new List<string>(); // 1
            List<string> retEmails = new List<string>(); // 2
            List<Fmd> retFmds = new List<Fmd>(); // 3
            List<int> retFingerIDs = new List<int>(); // 4
            List<string> retKeys = new List<string>(); // 5

            var users = from u in entities.users
                        where u.is_approved == 1 && u.is_lockedout == 0 && u.is_active == 1 &&
                        (u.finger_id1.HasValue || u.finger_id2.HasValue || u.finger_id3.HasValue || u.finger_id4.HasValue)
                        select u;

            if (users != null)
            {
                foreach (var u in users)
                {
                    if (u.finger_id1.HasValue)
                    {
                        retPKID.Add(u.pkid);
                        retUserName.Add(u.username);
                        retEmails.Add(u.email);
                        retKeys.Add(u.pkid);
                        try { retFmds.Add(Fmd.DeserializeXml(u.enrolled_fmd1)); }
                        catch { }
                        retFingerIDs.Add(u.finger_id1.Value);
                    }

                    if (u.finger_id2.HasValue)
                    {
                        retPKID.Add(u.pkid);
                        retUserName.Add(u.username);
                        retEmails.Add(u.email);
                        retKeys.Add(u.pkid);
                        try { retFmds.Add(Fmd.DeserializeXml(u.enrolled_fmd2)); }
                        catch { }
                        retFingerIDs.Add(u.finger_id2.Value);
                    }

                    if (u.finger_id3.HasValue)
                    {
                        retPKID.Add(u.pkid);
                        retUserName.Add(u.username);
                        retEmails.Add(u.email);
                        retKeys.Add(u.pkid);
                        try { retFmds.Add(Fmd.DeserializeXml(u.enrolled_fmd3)); }
                        catch { }
                        retFingerIDs.Add(u.finger_id3.Value);
                    }

                    if (u.finger_id4.HasValue)
                    {
                        retPKID.Add(u.pkid);
                        retUserName.Add(u.username);
                        retEmails.Add(u.email);
                        retKeys.Add(u.pkid);
                        try { retFmds.Add(Fmd.DeserializeXml(u.enrolled_fmd4)); }
                        catch { }
                        retFingerIDs.Add(u.finger_id4.Value);
                    }
                }
            }

            ret[0] = retPKID;
            ret[1] = retUserName;
            ret[2] = retEmails;
            ret[3] = retFmds;
            ret[4] = retFingerIDs;
            ret[5] = retKeys;
            return ret;

        }

    }
}
