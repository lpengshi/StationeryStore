using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class ReorderReportDTO
    {
        public LowStockDTO LowStockDTO { get; set; }
        public List<PurchaseOrderEF> Order { get; set; }
    }
}