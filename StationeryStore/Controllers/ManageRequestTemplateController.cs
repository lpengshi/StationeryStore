using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StationeryStore.Filters;
using StationeryStore.Models;
using StationeryStore.Service;

namespace StationeryStore.Controllers
{
    [AuthorizeFilter]
    [DepartmentFilter]
    public class ManageRequestTemplateController : Controller
    {
        StaffService staffService = new StaffService();
        StockService stockService = new StockService();
        RequestAndDisburseService rndService = new RequestAndDisburseService();

        [HttpGet]
        public ActionResult ViewRequestTemplate()
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;

            List<RequestTemplateEF> requestTemplate = rndService.FindRequestTemplateByStaffId(staff.StaffId);
            ViewBag.requestTemplate = requestTemplate;


            return View();
        }

        [HttpPost]

        public ActionResult ViewRequestTemplate(string templateName, string decision)
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;

            List<RequestTemplateEF> requestTemplate = rndService.FindRequestTemplateByStaffId(staff.StaffId);
            ViewBag.requestTemplate = requestTemplate;

            if (templateName == null || templateName == "")
            {
                ViewBag.note = "Please enter a valid template name";
            } else
            {
                rndService.CreateRequestTemplate(templateName, staff.StaffId);
                return RedirectToAction("ViewRequestTemplate");
            }

            return View();
        }


        [HttpGet]
        public ActionResult ManageRequestTemplate(int templateId)
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;

            RequestTemplateEF requestTemplate = rndService.FindRequestTemplateByTemplateId(templateId);

            if (requestTemplate.StaffId != staff.StaffId)
            {
                return RedirectToAction("ViewRequestTemplate");
            }

            RequestTemplateDTO requestTemplateDTO = rndService.FindRequestTemplateDetailsByTemplateId(templateId);

            List<CatalogueItemEF> catalogueList = stockService.ListCatalogueItems();
            ViewBag.catalogueList = catalogueList;

            return View(requestTemplateDTO);
        }

        [HttpPost]
        public ActionResult ManageRequestTemplate(RequestTemplateDTO requestTemplateDTO, string items, string decision)
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;
            List<CatalogueItemEF> catalogueList = stockService.ListCatalogueItems();
            ViewBag.catalogueList = catalogueList;

            if (decision == "Delete")
            {
                rndService.DeleteRequestTemplate(requestTemplateDTO.TemplateId);

                return RedirectToAction("ViewRequestTemplate");
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

                if (requestTemplateDTO.ItemDescription.Count > 0 && checkValid == true)
                {
                    foreach (var description in requestTemplateDTO.ItemDescription)
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
                    requestTemplateDTO = rndService.AddToRequestTemplateDTO(requestTemplateDTO, newItem.Stock.Description, newItem.Stock.Uom);
                }
            }

            if (decision == "Remove Selected Items")
            {
                int before = requestTemplateDTO.ItemDescription.Count;
                requestTemplateDTO = rndService.RemoveFromRequestTemplateDTO(requestTemplateDTO);
                int after = requestTemplateDTO.ItemDescription.Count;

                if (before != after)
                {
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.note = "Please check the remove box to remove selected row";
                }

            }

            if (decision == "Update")
            {
                if (requestTemplateDTO.ItemDescription.Count == 0)
                {
                    ViewBag.note = "You will need to have at least one item in the request";
                }
                else
                {
                    rndService.UpdateRequestTemplate(staff, requestTemplateDTO);
                    return RedirectToAction("ViewRequestTemplate");
                }   
            }

            if (decision == "Submit")
            {
                if (requestTemplateDTO.ItemDescription.Count == 0)
                {
                    ViewBag.note = "You will need to have at least one item in the request";
                }
                else
                {
                    RequestListDTO requestListDTO = rndService.ConvertToRequestListDTO(requestTemplateDTO);
                    rndService.CreateRequest(staff, requestListDTO);
                    return RedirectToAction("Index","ManageRequest",null);
                }
            }

            return View(requestTemplateDTO);
        }
    }
}