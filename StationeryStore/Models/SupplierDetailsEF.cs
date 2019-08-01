using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("SupplierDetails", Schema = "dbo")]
    public class SupplierDetailsEF
    {
        [Key]
        public int SupplierDetailsId { get; set; }

        [MaxLength(50)]
        public string SupplierCode { get; set; }
        [ForeignKey("SupplierCode")]
        public virtual SupplierEF Supplier { get; set; }

        [MaxLength(50)]
        public string ItemCode { get; set; }
        [ForeignKey("ItemCode")]
        public virtual StockEF Stock { get; set; }

        [Required]
        public double UnitPrice { get; set; }

        public int? SupplierRank { get; set; }
    }
}