using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class CatalogueItemDTO
    {
        [Display(Name ="Catalogue Id")]
        public int CatalogueId { get; set; }

        [Required]
        [Display(Name ="Item Code")]
        public string ItemCode { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(1, 1000)]
        [Display(Name = "Reorder Level")]
        public int ReorderLevel { get; set; }
        [Required]
        [Range(1, 1000)]
        [Display(Name = "Reorder Quantity")]
        public int ReorderQty { get; set; }
    }
}