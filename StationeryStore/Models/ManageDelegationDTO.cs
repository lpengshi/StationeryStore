using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    public class ManageDelegationDTO
    {
        public string DepartmentCode { get; set; }

        public int AuthorityId { get; set; }

        public DateTime DelegationStartDate { get; set; }

        public DateTime DelegationEndDate { get; set; }
    }
}