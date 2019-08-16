using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class MobileStationeryDisbursementDetailsDTO
    {
        public int DisbursementDetailsId { get; set; }
        public int DisbursementId { get; set; }
        public string ItemCode { get; set; }
        public StockEF Stock { get; set; }
        public int RequestQuantity { get; set; }
        public int RetrievedQuantity { get; set; }
        public int DisbursedQuantity { get; set; }
    }
}