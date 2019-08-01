using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("StationeryDisbursement", Schema = "dbo")]
    public class StationeryDisbursementEF
    {
        [Key]
        public int DisbursementId { get; set; }

        public int RetrievalId { get; set; }
        [ForeignKey("RetrievalId")]
        public virtual StationeryRetrievalEF StationeryRetrieval { get; set; }

        [MaxLength(50)]
        public string DepartmentCode { get; set; }
        [ForeignKey("DepartmentCode")]
        public virtual DepartmentEF Department { get; set; }

        public long? DateDisbursed { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        public int? CollectionRepId { get; set; }
        [ForeignKey("CollectionRepId")]
        public virtual StaffEF Staff { get; set; }

        public virtual ICollection<StationeryDisbursementDetailsEF> StationeryDisbursementDetails { get; set; }
    }
}