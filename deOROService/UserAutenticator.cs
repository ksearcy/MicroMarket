using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using deORODataAccess;

namespace deOROService
{
    public class UserAutenticator : System.IdentityModel.Selectors.UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            LocationRepository locRepo = new LocationRepository();

            if (userName == null || password == null)
            {
                throw new FaultException("UserName or Password is null");
            }

            if (!locRepo.ValidateUser(userName,password))
            {
                throw new FaultException("Incorrect Username or Password");
            }
        }
    }
}