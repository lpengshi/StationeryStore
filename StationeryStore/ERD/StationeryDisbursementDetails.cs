//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StationeryStore.ERD
{
    using System;
    using System.Collections.Generic;
    
    public partial class StationeryDisbursementDetails
    {
        public int DisbursementDetailsId { get; set; }
        public int DisbursementId { get; set; }
        public string ItemCode { get; set; }
        public int RequestQuantity { get; set; }
        public int RetrievedQuantity { get; set; }
        public int DisbursedQuantity { get; set; }
    
        public virtual StationeryDisbursement StationeryDisbursement { get; set; }
        public virtual Stock Stock { get; set; }
    }
}