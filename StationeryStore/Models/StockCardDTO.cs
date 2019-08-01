using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class StockCardDTO
    {
        [Required]
        [MaxLength(10)]
        [Display(Name = "Item Code")]
        public string ItemCode { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; }

        [Required]
        [MaxLength(50)]
        public string Description { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "UOM")]
        public string Uom { get; set; }

        [Required]
        [MaxLength(10)]
        [Display(Name = "Bin No.")]
        public string Bin { get; set; }

        [Display(Name ="Quantity On Hand")]
        public int QuantityOnHand { get; set; }
    }
}