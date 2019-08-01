using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class RequestDTO
    {
        public string RequestId { get; set; }
        public string RequestDate { get; set; }
        public string Status { get; set; }

        public RequestDTO() {; }

        public RequestDTO(string requestId, string requestDate, string status)
        {
            RequestId = requestId;
            RequestDate = requestDate;
            Status = status;
        }
    }
}