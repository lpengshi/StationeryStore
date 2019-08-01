using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("PurchaseOrderDetails", Schema = "dbo")]
    public class PurchaseOrderDetailsEF
    {
        [Key]
        public int OrderDetailsId { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual PurchaseOrderEF PurchaseOrder { get; set; }

        [MaxLength(50)]
        public string ItemCode { get; set; }
        [ForeignKey("ItemCode")]
        public virtual StockEF Stock { get; set; }

        public int QuantityOrdered { get; set; }
    }
}
