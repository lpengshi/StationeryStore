//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StationeryStore
{
    using System;
    using System.Collections.Generic;
    
    public partial class StationeryRequestDetails
    {
        public int RequestDetailsId { get; set; }
        public string RequestId { get; set; }
        public string ItemCode { get; set; }
        public int RequestQuantity { get; set; }
        public string FulfilmentStatus { get; set; }
        public int FulfilledQuantity { get; set; }
        public Nullable<int> RetrievedQuantity { get; set; }
    
        public virtual StationeryRequest StationeryRequest { get; set; }
        public virtual Stock Stock { get; set; }
    }
}
