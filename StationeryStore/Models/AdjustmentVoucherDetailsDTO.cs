using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class AdjustmentVoucherDetailsDTO
    {
        [Required]
        public string ItemCode { get; set; }
        public string Description { get; set;}

        [Range(-9999, 9999)]
        public int Quantity { get; set; }

        [Required]
        public string Reason { get; set; }
        public bool Remove { get; set; }
    }
}