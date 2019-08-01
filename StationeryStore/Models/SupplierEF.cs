using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("Supplier", Schema = "dbo")]
    public class SupplierEF
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public string SupplierCode { set; get; }

        [MaxLength(255)]
        public string SupplierName { set; get; }

        [MaxLength(255)]
        public string ContactName { set; get; }

        [MaxLength(50)]
        public string PhoneNo { set; get; }

        [MaxLength(50)]
        public string FaxNo { set; get; }

        [MaxLength(255)]
        public string Address { set; get; }

        [MaxLength(255)]
        public string GstRegistrationNo { set; get; }

        public virtual ICollection<SupplierDetailsEF> SupplierDetails { get; set; }
        public virtual ICollection<PurchaseOrderEF> PurchaseOrders { get; set; }
    }
}