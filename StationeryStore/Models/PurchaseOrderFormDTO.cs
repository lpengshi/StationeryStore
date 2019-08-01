using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class PurchaseOrderFormDTO
    {
        [Required]
        public string DeliveryAdd { get; set; }

        [DataType(DataType.Date), Required]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime SupplyItemBy { get; set; }

        public string Description { get; set; }

        public string SupplierId { get; set; }

        public List<string> Icodes { get; set; }
        public List<string> Descs { get; set; }
        public List<string> Uoms { get; set; }
        public List<double> Prices { get; set; }
        public List<string> SupplierDetailIds { get; set; }


        public List<int> Quantities { get; set; }

        public List<bool> Remove { get; set; }

        public PurchaseOrderFormDTO()
        {
             SupplierDetailIds = new List<string>();
             Quantities = new List<int>();
             Descs = new List<string>();
            Uoms = new List<string>();
            Prices = new List<double>();
            Icodes = new List<string>();
            Remove = new List<bool>();
        }

    }
}