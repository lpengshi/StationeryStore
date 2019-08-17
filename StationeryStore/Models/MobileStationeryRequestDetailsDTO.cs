using StationeryStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class MobileStationeryRequestDetailsDTO
    {
        public int RequestDetailsId { get; set; }
        public string RequestId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int RequestQuantity { get; set; }
    }
}