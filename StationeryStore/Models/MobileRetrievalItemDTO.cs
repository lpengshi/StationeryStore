using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class MobileRetrievalItemDTO
    {
        public int RetrievalId { get; set; }
        public String DateDisbursed { get; set; }
        public List<RetrievalItemDTO> RetrievalItems { get; set; }
    }
}