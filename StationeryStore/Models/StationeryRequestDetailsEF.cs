using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("StationeryRequestDetails", Schema = "dbo")]
    public class StationeryRequestDetailsEF
    {
        [Key]
        public int RequestDetailsId { get; set; }

        [MaxLength(50)]
        public string RequestId { get; set; }
        [ForeignKey("RequestId")]
        public virtual StationeryRequestEF Request { get; set; }

        [MaxLength(50)]
        public string ItemCode { get; set; }
        [ForeignKey("ItemCode")]
        public virtual StockEF Stock { get; set; }

        public int RequestQuantity { get; set; }

        [MaxLength(50)]
        public string FulfilmentStatus { get; set; }

        public int FulfilledQuantity { get; set; }

        // for a specific round of retrieval, set to null once done
        public int? RetrievedQuantity { get; set; }

        public StationeryRequestDetailsEF()
        {

        }
        public StationeryRequestDetailsEF(string requestId, string itemCode, int requestQuantity)
        {
            RequestId = requestId;
            ItemCode = itemCode;
            RequestQuantity = requestQuantity;
            FulfilmentStatus = "Submitted";
        }
    }
}