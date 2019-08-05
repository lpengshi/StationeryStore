using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class InventoryStatusReportDTO
    {
        public StockEF Stock { get; set; }
        
        // can  be null for stock items not in catalogue
        public CatalogueItemEF CatalogueItem { get; set; }
    }
}