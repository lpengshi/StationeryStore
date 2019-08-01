using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class RequestListDTO
    {
        public List<string> ItemDescription { get; set; }

        public List<string> ItemUom { get; set; }

        public List<int> Quantity { get; set; }

        public List<bool> Remove { get; set; }

        public string Status { get; set; }

        public string RequestId { get; set; }

        public RequestListDTO()
        {
            ItemDescription = new List<string>();
            ItemUom = new List<string>();
            Quantity = new List<int>();
            Remove = new List<bool>();

        }
    }
}