using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using StationeryStore.Models;
using System.Threading.Tasks;

namespace StationeryStore.Service
{
    public class PredictService
    {
        StockService stockService = new StockService();

       public async Task<int> PredictReorderQuantity(CatalogueItemDTO item)
        {
            using (var client = new HttpClient())
            {
                int result = -1;

                StockEF predictItem = stockService.FindStockByItemCode(item.ItemCode);

                int category = LabelEncodeByCategory(predictItem.Category);

                int popularity = 1; //to access past data and determine popularity in the month

                int uom = LabelEncodeByUOM(predictItem.Uom);

                PredictReorderQtyDTO predModel = new PredictReorderQtyDTO(category, popularity, uom);

                // send a POST request to the server uri with the data and get the response as HttpResponseMessage object
                HttpResponseMessage res = await client.PostAsJsonAsync("http://127.0.0.1:5000/", predModel);

                // Return the result from the server if the status code is 200 (everything is OK)
                if (res.IsSuccessStatusCode)
                {
                    result = int.Parse(res.Content.ReadAsStringAsync().Result);
                }

                return result;
            }
        }

        public int LabelEncodeByUOM(string uom)
        {
            int uomLabel = -1;

            switch (uom)
            {
                case "Each":
                    uomLabel = 0;
                    break;
                case "Packet":
                    uomLabel = 1;
                    break;
                case "Box":
                    uomLabel = 2;
                    break;
                case "Set":
                    uomLabel = 3;
                    break;
                case "Dozen":
                    uomLabel = 4;
                    break;
            }

            return uomLabel;
        }

        public int LabelEncodeByCategory(string category)
        {
            int categoryLabel = -1;
            switch (category)
            {
                case "Clip":
                    categoryLabel = 0;
                    break;
                case "Envelope":
                    categoryLabel = 1;
                    break;
                case "Eraser":
                    categoryLabel = 2;
                break;
                case "Exercise":
                    categoryLabel = 3;
                    break;
                case "File":
                    categoryLabel = 4;
                    break;
                case "Pen":
                    categoryLabel = 5;
                    break;
                case "Puncher":
                    categoryLabel = 6;
                    break;
                case "Pad":
                    categoryLabel = 7;
                    break;
                case "Paper":
                    categoryLabel = 8;
                    break;
                case "Ruler":
                    categoryLabel = 9;
                    break;
                case "Scissors":
                    categoryLabel = 10;
                    break;
                case "Tape":
                    categoryLabel = 11;
                    break;
                case "Sharpener":
                    categoryLabel = 12;
                    break;
                case "Shorhand":
                    categoryLabel = 13;
                    break;
                case "Stapler":
                    categoryLabel = 14;
                    break;
                case "Tacks":
                    categoryLabel = 15;
                    break;
                case "Tparency":
                    categoryLabel = 16;
                    break;
                case "Tray":
                    categoryLabel = 17;
                    break;
            }

            return categoryLabel;
        }
    }
}