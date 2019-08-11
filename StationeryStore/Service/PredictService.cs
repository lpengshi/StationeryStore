using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using StationeryStore.Models;
using System.Threading.Tasks;
using StationeryStore.Util;
using StationeryStore.EntityFrameworkFacade;

namespace StationeryStore.Service
{
    public class PredictService
    {
        RequestAndDisburseEFFacade rndEFF = new RequestAndDisburseEFFacade();
        StockEFFacade stockEFF = new StockEFFacade();

       public PredictReorderQtyDTO GetPredictModel(CatalogueItemDTO item)
        {
            using (var client = new HttpClient())
            {
                StockEF predictItem = stockEFF.FindStockByItemCode(item.ItemCode);

                //label encoding for the three features
                //contain the label encoding for category and also the average quantity based on category and uom
                int[] categoryAvgQty = LabelEncodeByCategory(predictItem.Category, predictItem.Uom);
                int uom = LabelEncodeByUOM(predictItem.Uom);
                int popularity = LabelEncodeByPopularity(predictItem.ItemCode, categoryAvgQty[1], uom);

                PredictReorderQtyDTO predModel = new PredictReorderQtyDTO(categoryAvgQty[0], popularity, uom);

                return predModel;
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

        public int[] LabelEncodeByCategory(string category, string uom)
        {
            int[] categoryAvgQty = new int[2];

            int categoryLabel = -1;
            int averageQty = -1;
            switch (category)
            {
                case "Clip":
                    categoryLabel = 0;
                    averageQty = 30;
                    break;
                case "Envelope":
                    categoryLabel = 1;
                    averageQty = 400;
                    break;
                case "Eraser":
                    categoryLabel = 2;
                    averageQty = 20;
                    break;
                case "Exercise":
                    categoryLabel = 3;
                    averageQty = 40;
                    break;
                case "File":
                    categoryLabel = 4;
                    if (uom == "Set")
                    {
                        averageQty = 40;
                    } else
                    {
                        averageQty = 200;
                    }
                    break;
                case "Pen":
                    categoryLabel = 5;
                    if (uom == "Box")
                    {
                        averageQty = 80;
                    } else
                    {
                        averageQty = 40;
                    }
                    break;
                case "Puncher":
                    categoryLabel = 6;
                    averageQty = 20;
                    break;
                case "Pad":
                    categoryLabel = 7;
                    averageQty = 80;
                    break;
                case "Paper":
                    categoryLabel = 8;
                    averageQty = 400;
                    break;
                case "Ruler":
                    categoryLabel = 9;
                    averageQty = 20;
                    break;
                case "Scissors":
                    categoryLabel = 10;
                    averageQty = 20;
                    break;
                case "Tape":
                    categoryLabel = 11;
                    averageQty = 20;
                    break;
                case "Sharpener":
                    categoryLabel = 12;
                    averageQty = 20;
                    break;
                case "Shorthand":
                    categoryLabel = 13;
                    averageQty = 80;
                    break;
                case "Stapler":
                    categoryLabel = 14;
                    averageQty = 20;
                    break;
                case "Tacks":
                    categoryLabel = 15;
                    averageQty = 20;
                    break;
                case "Tparency":
                    categoryLabel = 16;
                    averageQty = 400;
                    break;
                case "Tray":
                    categoryLabel = 17;
                    averageQty = 20;
                    break;
            }

            categoryAvgQty[0] = categoryLabel;
            categoryAvgQty[1] = averageQty;

            return categoryAvgQty;
        }

        public int LabelEncodeByPopularity(string itemCode, int averageQty, int uom) {

            int popularityLabel = 1;

            DateTime prevYearDate = DateTime.UtcNow.AddYears(-1);

            DateTime firstDayOfMonth = new DateTime(prevYearDate.Year, prevYearDate.Month, 1);
            long startMonth = (long)(firstDayOfMonth.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            long endMonth = (long)(lastDayOfMonth.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            int disbursedQty =  rndEFF.FindDisbursedQuantityByItemCodeAndTimePeriod(itemCode, startMonth, endMonth);

            if (disbursedQty > 0)
            {
                if (disbursedQty < 0.5 * averageQty)
                {
                    popularityLabel = 0;
                } else if (disbursedQty > averageQty)
                {
                    popularityLabel = 2;
                }
            }

            return popularityLabel;
        }
    }
}