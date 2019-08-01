using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("PurchaseOrder", Schema = "dbo")]
    public class PurchaseOrderEF
    {
        [Key]
        public int OrderId { get; set; }

        public long OrderDate { get; set; }

        public int? CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public virtual StaffEF CreatedBy { get; set; }

        public string SupplierCode { get; set; }
        [ForeignKey("SupplierCode")]
        public virtual SupplierEF Supplier { get; set; }

        [MaxLength(255)]
        public string DeliveryAddress { get; set; }

        public long DeliverByDate { get; set; }
        public long DateDeliveredOn { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        public int? ReceivedById { get; set; }
        [ForeignKey("ReceivedById")]
        public virtual StaffEF ReceivedBy { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        public virtual ICollection<PurchaseOrderDetailsEF> PurchaseOrderDetails { get; set; }
    }
}