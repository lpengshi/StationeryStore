using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("Stock", Schema = "dbo")]
    public class StockEF
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public string ItemCode { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Uom { get; set; }

        [MaxLength(50)]
        public string Bin { get; set; }

        public int QuantityOnHand { get; set; }

    }
}