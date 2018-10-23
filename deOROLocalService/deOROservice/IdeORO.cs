using System;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace deOROservice
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IdeORO" in both code and config file together.
    [ServiceContract]
    public interface IdeORO
    {
        #region User Log In Interface

        [OperationContract]
        [WebInvoke(Method = "POST",
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped,
                   UriTemplate = "/LogIn"
                  )
        ]
        UserData LogIn(Stream ReceiveData);

        #endregion

        #region Get User Profile Interface

        [OperationContract]
        [WebInvoke(Method = "POST",
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped,
                   UriTemplate = "/GetUserProfile"
                  )
        ]
        UserData GetUserProfile(Stream ReceiveData);

        #endregion

        #region Update User Profile Interface

        [OperationContract]
        [WebInvoke(Method = "POST",
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped,
                   UriTemplate = "/UpdateUserProfile"
                  )
        ]
        UserData UpdateUserProfile(Stream ReceiveData);

        #endregion

        #region Log Out Interface

        [OperationContract]
        [WebInvoke(Method = "POST",
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped,
                   UriTemplate = "/LogOut"
                  )
        ]
        UserData LogOut(Stream ReceiveData);

        #endregion

        #region Delete User Interface

        [OperationContract]
        [WebInvoke(Method = "POST",
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped,
                   UriTemplate = "/DeleteUser"
                  )
        ]
        UserData DeleteUser(Stream ReceiveData);

        #endregion
    }

    #region User Data Contract

    [DataContract]
    public class UserData
    {
        [DataMember]
        public string Device { get; set; }
        [DataMember]
        public string DeviceId { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Function { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string ApiKey { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string CustomerId { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string TotalBalance { get; set; }

        public UserData()
        {
            Device = "";
            DeviceId = "";
            Type = "";
            Function = "";
            Status = "";
            Message = "";
            ApiKey = "";
            UserName = "";
            Password = "";
            CustomerId = "";
            Email = "";
            FirstName = "";
            LastName = "";
            TotalBalance = "";
        }
    }

    #endregion
}
