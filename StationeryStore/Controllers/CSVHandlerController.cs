using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using StationeryStore.Models;
using StationeryStore.Service;

namespace StationeryStore.Util
{
    public class CSVHandlerController : Controller
    {
        PurchaseService purchaseService = new PurchaseService();

        //SUPPLIER DETAILS IMPORT + EXPORT
        [HttpPost]
        public ActionResult ImportSupplierDetails(HttpPostedFileBase fileUploaded)
        {
            try
            {
                //Check if there is a file is sent to the controller
                if (fileUploaded != null && fileUploaded.ContentLength > 0)
                {
                    // Check if the file ends with csv extention
                    if (fileUploaded.FileName.EndsWith(".csv"))
                    {
                        //// Instance of EF Class;
                        //DBContext db = new DBContext();
                        //Validation_CVS csvValidate = new Validation_CVS(); //Instance of Validation Class

                        // Read the file as a stream
                        StreamReader streamCsv = new StreamReader(fileUploaded.InputStream);

                        string csvDataLine = "";
                        int CurrentLine = 0;

                        string[] LineData = null;

                        // Delete all data everytime a new file is uploaded.
                        purchaseService.ClearSupplierDetailsData();

                        // Looping to read the File stream and Add Data to the database line by line
                        while ((csvDataLine = streamCsv.ReadLine()) != null)
                        {
                            // Ignore the first line of the file, for column names.
                            if (CurrentLine != 0)
                            {
                                // Validate File Data and normalize it
                                //csvDataLine = csvValidate.Validate(csvDataLine);

                                // Add the returned data to an array
                                LineData = csvDataLine.Split(',');

                                //Pass LineData array values
                                var newSupplierDetail = new SupplierDetailsEF()
                                {
                                    SupplierDetailsId = int.Parse(LineData[0]),
                                    SupplierCode = LineData[1],
                                    ItemCode = LineData[2],
                                    UnitPrice = double.Parse(LineData[3]),
                                    SupplierRank = int.Parse(LineData[4])
                                };

                                //Add Data to The Database                              
                                purchaseService.AddSupplierDetail(newSupplierDetail);

                            }
                            CurrentLine += 1;
                        }
                        Debug.Print("Loaded: " + (CurrentLine - 1) + " items to supplier details table.");
                    }
                    else
                        TempData["MessageError"] = "File Format is not Supported";
                }
                else
                    TempData["MessageError"] = "Please Upload A File";

                // Back to the first view
                return RedirectToAction("Index", "ManageSupplier");

            }
            catch (Exception ex)
            {
                TempData["MessageError"] = "Error:" + ex.Message;
                return RedirectToAction("Index", "ManageSupplier");

            }
        }

        [HttpPost]
        public FileResult ExportSupplierDetailsToCSV()
        {
            List<SupplierDetailsEF> sdef = purchaseService.FindAllSupplierDetails();
            List<object> supplierDetails = (from sd in sdef
                                            select new[] {
                                                sd.SupplierDetailsId.ToString(),
                                                sd.SupplierCode,
                                                sd.ItemCode,
                                                sd.UnitPrice.ToString(),
                                                sd.SupplierRank.ToString()
                                            }).ToList<object>();

            //Insert column names
            supplierDetails.Insert(0, new string[5] { "SupplierDetailsId", "SupplierCode", "ItemCode", "UnitPrice", "SupplierRank"});

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < supplierDetails.Count; i++)
            {
                string[] detail = (string[])supplierDetails[i];
                for (int j = 0; j < detail.Length; j++)
                {
                    //Append data with separator.
                    sb.Append(detail[j] + ',');
                }
                    
                //Append new line character.
                sb.Append("\r\n");

            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "SupplierDetails.csv");
        }
    }
}
