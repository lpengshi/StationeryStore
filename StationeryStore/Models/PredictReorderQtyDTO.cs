using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class PredictReorderQtyDTO
    {
        public int Category { get; set; }

        public int Popularity { get; set; }

        public int UOM { get; set; }

        public PredictReorderQtyDTO()
        {
        }

        public PredictReorderQtyDTO (int category, int popularity, int uom)
        {
            Category = category;
            Popularity = popularity;
            UOM = uom;
        }
    }
}