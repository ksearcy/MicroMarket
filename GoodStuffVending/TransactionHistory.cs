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
    
    public partial class TransactionHistory
    {
        public int historyID { get; set; }
        public System.DateTime dateOfPurchase { get; set; }
        public Nullable<bool> cash { get; set; }
        public Nullable<bool> credit { get; set; }
        public Nullable<bool> debit { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> tax { get; set; }
        public Nullable<decimal> crv { get; set; }
        public string userID { get; set; }
        public string what { get; set; }
        public Nullable<bool> isError { get; set; }
        public string errorMessage { get; set; }
        public int kioskID { get; set; }
        public string transactionID { get; set; }
    }
}