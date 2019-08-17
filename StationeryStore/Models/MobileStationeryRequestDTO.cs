using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class MobileStationeryRequestDTO
    {
        public string RequestId { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string Status { get; set; }
        public long RequestDate { get; set; }
        public int DecisionById { get; set; }
        public string DecisionByName { get; set; }
        public long DecisionDate { get; set; }
        public string Comment { get; set; }
        public string Decision { get; set; }
    }
}