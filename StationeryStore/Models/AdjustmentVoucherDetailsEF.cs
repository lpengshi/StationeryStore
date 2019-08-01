using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("AdjustmentVoucherDetails", Schema = "dbo")]
    public class AdjustmentVoucherDetailsEF
    {
        [Key]
        public int AdjustmentId { get; set; }

        public string VoucherId { get; set; }
        [ForeignKey("VoucherId")]
        public virtual AdjustmentVoucherEF Voucher { get; set; }

        [MaxLength(50)]
        public string ItemCode { get; set; }
        [ForeignKey("ItemCode")]
        public virtual StockEF Stock { get; set; }

        public int Quantity { get; set; }

        [MaxLength(255)]
        public string Reason { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }
    }
}