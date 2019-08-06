using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("RequestTemplateDetails", Schema = "dbo")]
    public class RequestTemplateDetailsEF
    {
        [Key]
        public int TemplateDetailsId { get; set; }

        public int RequestTemplateId { get; set; }
        [ForeignKey("RequestTemplateId")]
        public virtual RequestTemplateEF RequestTemplate { get; set; }

        [MaxLength(50)]
        public string ItemCode { get; set; }
        [ForeignKey("ItemCode")]
        public virtual StockEF Stock { get; set; }

        public int RequestQuantity { get; set; }

        public RequestTemplateDetailsEF()
        {

        }
        public RequestTemplateDetailsEF(int templateId, string itemCode, int requestQuantity)
        {
            RequestTemplateId = templateId;
            ItemCode = itemCode;
            RequestQuantity = requestQuantity;
        }
    }
}