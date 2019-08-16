using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class MobileDisbursementItemDTO
    {
        public List<MobileActiveDepartmentDTO> ActiveDepartments { get; set; }
        //public StationeryDisbursementEF StationeryDisbursement { get; set; }
        public List<MobileStationeryDisbursementDetailsDTO> DisbursementDetails { get; set; }
        public List<MobileStaffDTO> DepartmentStaff { get; set; }
        public int ClerkId { get; set; }
        public int DisbursementId { get; set; }
        public int CollectionRepId { get; set; }
    }
}