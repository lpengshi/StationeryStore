using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("StockTransactionDetails", Schema = "dbo")]
    public class StockTransactionDetailsEF
    {
        [Key]
        public int StockTransDetailId { get; set; }

        [MaxLength(50)]
        public string ItemCode { get; set; }
        [ForeignKey("ItemCode")]
        public virtual StockEF Stock { get; set; }

        public long Date { get; set; }

        public int Quantity { get; set; }

        public int Balance { get; set; }

        [MaxLength(50)]
        public string Type { get; set; }

    }
}