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
    
    public partial class PurchaseOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PurchaseOrder()
        {
            this.PurchaseOrderDetails = new HashSet<PurchaseOrderDetails>();
        }
    
        public int OrderId { get; set; }
        public long OrderDate { get; set; }
        public Nullable<int> CreatedById { get; set; }
        public string SupplierCode { get; set; }
        public string DeliveryAddress { get; set; }
        public long DeliverByDate { get; set; }
        public long DateDeliveredOn { get; set; }
        public string Description { get; set; }
        public Nullable<int> ReceivedById { get; set; }
        public string Status { get; set; }
    
        public virtual Staff Staff { get; set; }
        public virtual Staff Staff1 { get; set; }
        public virtual Supplier Supplier { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrderDetails> PurchaseOrderDetails { get; set; }
    }
}
