using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class RetrievalItemDTO
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Bin { get; set; }
        public int TotalOutstandingQty { get; set; }
        [Required]
        public int? RetrievedQty { get; set; }
    }
}