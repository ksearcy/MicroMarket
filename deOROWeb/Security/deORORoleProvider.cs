using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using deORODataAccess;

namespace deOROWeb.Security
{
    public class deORORoleProvider : RoleProvider
    {
        WebUserRepository repo = new WebUserRepository();

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            var user = repo.FindBy(x => x.username == username).SingleOrDefault();

            if (user != null)
            {
                if (user.is_admin == 1)
                {
                    return new[] { "Admin" };
                }
                else
                {
                    return new[] { "User" };
                }
            }

            return new[] { "" };
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var user = repo.FindBy(x => x.is_admin == (roleName == "Admin" ? 1 : 0)).SingleOrDefault();

            if (user != null)
                return true;
            else
                return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}