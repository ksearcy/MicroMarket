//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace deORODataAccessApp
{
    using System;
    using System.Collections.Generic;
    
    public partial class accountbalancehistory
    {
        public int id { get; set; }
        public string pkid { get; set; }
        public string userpkid { get; set; }
        public string shoppingcartpkid { get; set; }
        public Nullable<decimal> account_balance { get; set; }
        public Nullable<decimal> amount { get; set; }
        public string description { get; set; }
        public Nullable<System.DateTime> created_date_time { get; set; }
    }
}
