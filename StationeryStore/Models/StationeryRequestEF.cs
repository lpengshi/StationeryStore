using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("StationeryRequest", Schema = "dbo")]
    public class StationeryRequestEF
    {
        [Key]
        [MaxLength(50)]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public string RequestId { get; set; }

        public int StaffId { get; set; }
        [ForeignKey("StaffId")]
        public virtual StaffEF Staff { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        public long RequestDate { get; set; }

        public int? DecisionById { get; set; }
        [ForeignKey("DecisionById")]
        public virtual StaffEF DecisionBy { get; set; }

        public long? DecisionDate { get; set; }

        public string Designation { get; set; }

        [MaxLength(255)]
        public string Comment { get; set; }

        public StationeryRequestEF()
        {

        }
        public StationeryRequestEF(string requestId, int staffId, long requestDate)
        {
            RequestId = requestId;
            StaffId = staffId;
            RequestDate = requestDate;
            Status = "Submitted";
        }
    }
}