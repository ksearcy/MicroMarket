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
    
    public partial class discount
    {
        public int id { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public Nullable<System.DateTime> date_from { get; set; }
        public Nullable<System.DateTime> date_to { get; set; }
        public string time_from { get; set; }
        public string time_to { get; set; }
        public Nullable<byte> monday { get; set; }
        public Nullable<byte> tuesday { get; set; }
        public Nullable<byte> wednesday { get; set; }
        public Nullable<byte> thursday { get; set; }
        public Nullable<byte> friday { get; set; }
        public Nullable<byte> saturday { get; set; }
        public Nullable<byte> sunday { get; set; }
        public Nullable<decimal> percent { get; set; }
        public Nullable<decimal> amount { get; set; }
        public Nullable<byte> is_active { get; set; }
    }
}
