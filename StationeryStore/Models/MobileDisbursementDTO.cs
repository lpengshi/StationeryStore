using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class MobileDisbursementDTO
    {
        public StationeryDisbursementEF Disbursement { get; set; }
        public List<StationeryDisbursementDetailsEF> DisbursementDetails { get; set; }
    }
}