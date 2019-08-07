using System;
using System.Collections.Generic;
using System.Web.Mvc;
using StationeryStore.Filters;
using StationeryStore.Models;
using StationeryStore.Service;
using StationeryStore.Util;

namespace StationeryStore.Controllers
{
    [AuthorizeFilter]
    [DepartmentFilter]
    public class ManageRequestController : Controller
    {
        StaffService staffService = new StaffService();
        StockService stockService = new StockService();
        RequestAndDisburseService rndService = new RequestAndDisburseService();

        public ActionResult Index()
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;
            List<RequestDTO> requestDTOList = rndService.FindRequestByStaffAndStatus(staff.StaffId, "Submitted");

            ViewBag.requestDTOList = requestDTOList;

            return View();
        }

        [HttpGet]
        public ActionResult CreateRequest()
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;

            List<CatalogueItemEF> catalogueList = stockService.ListCatalogueItems();
            ViewBag.catalogueList = catalogueList;
            RequestListDTO requestListDTO = null;

            if (TempData["requestListDTO"] != null)
            {
                requestListDTO = (RequestListDTO)TempData["requestListDTO"];
                ViewBag.amend = true;
            }

            return View(requestListDTO);
        }

        [HttpPost]
        public ActionResult CreateRequest(RequestListDTO requestListDTO, string items, string decision)
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;
            List<CatalogueItemEF> catalogueList = stockService.ListCatalogueItems();
            ViewBag.catalogueList = catalogueList;

            if (requestListDTO.RequestId != null)
            {
                ViewBag.amend = true;
            }

            if (decision == "Delete")
            {
                rndService.DeleteRequest(requestListDTO.RequestId);

                return RedirectToAction("Index");
            }

            if (decision == null)
            {
                ViewBag.note = "Invalid quantity detected. Please amend.";
            }

            if (items != null && decision == "Add Item")
            {
                CatalogueItemEF newItem = null;
                bool checkValid = false;
                foreach (var item in catalogueList)
                {
                    if (item.Stock.Description == items)
                    {
                        checkValid = true;
                        newItem = item;
                        break;
                    } 
                }

                if (!checkValid)
                {
                    ViewBag.note = "The item is not recognised. Please select another item from the list";
                }

                if (requestListDTO.ItemDescription.Count > 0 && checkValid == true)
                {
                    foreach (var description in requestListDTO.ItemDescription)
                    {
                        if (description == items)
                        {
                            checkValid = false;
                            ViewBag.note = "The item has already been added to the request list";
                            break;
                        } 
                    }
                }
                if (checkValid)
                {
                    requestListDTO = rndService.AddToRequestListDTO(requestListDTO, newItem.Stock.Description, newItem.Stock.Uom);
                }
            }

            if (decision == "Remove Selected Items")
            {
                int before = requestListDTO.ItemDescription.Count;
                requestListDTO = rndService.RemoveFromRequestListDTO(requestListDTO);
                int after = requestListDTO.ItemDescription.Count;

                if (before != after)
                {
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.note = "Please check the remove box to remove selected row";
                }
                  
            }

            if (decision == "Submit" || decision == "Update")
            {
                if (requestListDTO.ItemDescription.Count == 0)
                {
                    ViewBag.note = "You will need to have at least one item in the request";
                } else
                {
                    rndService.CreateRequest(staff, requestListDTO);
                    return RedirectToAction("Index");
                }
            }
            return View(requestListDTO);
        }

        public ActionResult ViewRequest(string requestId)
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;
            StationeryRequestEF request = rndService.FindRequestById(requestId);
            List<StationeryRequestDetailsEF> requestDetails = rndService.FindRequestDetailsByRequestId(requestId);

            if (request.Status == "Submitted")
            {
                RequestListDTO requestListDTO = rndService.AddToRequestListDTO(requestId, request.Status, requestDetails);
                TempData["requestListDTO"] = requestListDTO;

                return RedirectToAction("CreateRequest");
            }
            ViewBag.request = request;
            ViewBag.requestDetails = requestDetails;
            
            ViewBag.requestdate = Timestamp.dateFromTimestamp(request.RequestDate);

            long decisionDate = request.DecisionDate.GetValueOrDefault();
            ViewBag.decisiondate = Timestamp.dateFromTimestamp(decisionDate);

            return View();
        }

    }
}