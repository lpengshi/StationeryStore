using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class SupplierDTO
    {
        [Required]
        public string SupplierCode { get; set; }

        [Required]
        public string SupplierName { get; set; }

        [Required]
        public string GstNumber { get; set; }

        [Required]
        public string Contact { get; set; }

        [Required]
        public string PhoneNo { get; set;}

        [Required]
        public string FaxNo { get; set; }

        [Required]
        public string Address { get; set; }

    }
}