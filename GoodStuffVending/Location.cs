//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GoodStuffVending
{
    using System;
    using System.Collections.Generic;
    
    public partial class Location
    {
        public int kioskID { get; set; }
        public string location1 { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public Nullable<int> zip { get; set; }
        public Nullable<int> phone { get; set; }
        public string contact1 { get; set; }
        public string contact2 { get; set; }
        public string description { get; set; }
        public System.Guid talkID { get; set; }
        public string merchantID { get; set; }
        public string vUser { get; set; }
        public string nPass { get; set; }
        public Nullable<System.Guid> cshID { get; set; }
        public string SecureLink { get; set; }
        public string resetPassword { get; set; }
        public Nullable<System.DateTime> lastResetTime { get; set; }
        public string resetStatus { get; set; }
    }
}