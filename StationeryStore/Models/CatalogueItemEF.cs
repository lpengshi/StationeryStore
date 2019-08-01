using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("CatalogueItem", Schema = "dbo")]
    public class CatalogueItemEF
    {
        [Key]
        public int CatalogueId { get; set; }

        [MaxLength(50)]
        public string ItemCode { get; set; }
        [ForeignKey("ItemCode")]
        public virtual StockEF Stock { get; set; }

        public int ReorderLevel { get; set; }

        public int ReorderQty { get; set; }
    }
}