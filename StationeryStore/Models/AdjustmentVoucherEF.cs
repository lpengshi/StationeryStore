using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("AdjustmentVoucher", Schema = "dbo")]
    public class AdjustmentVoucherEF
    {
        [Key]
        public string VoucherId { get; set; }

        public int? ApproverId { get; set; }
        [ForeignKey("ApproverId")]
        public virtual StaffEF Approver { get; set; }

        public long DateIssued { get; set; }

        public string Status { get; set; }

        public virtual ICollection<AdjustmentVoucherDetailsEF> AdjustmentVoucherDetails { get; set; }
    }
}