using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("StationeryDisbursementDetails", Schema = "dbo")]
    public class StationeryDisbursementDetailsEF
    {
        [Key]
        public int DisbursementDetailsId { get; set; }

        public int DisbursementId { get; set; }
        [ForeignKey("DisbursementId")]
        public virtual StationeryDisbursementEF StationeryDisbursement { get; set; }

        public string ItemCode { get; set; }
        [ForeignKey("ItemCode")]
        public virtual StockEF Stock { get; set; }

        public int RequestQuantity { get; set; }
        public int RetrievedQuantity { get; set; }
        public int DisbursedQuantity { get; set; }
    }
}