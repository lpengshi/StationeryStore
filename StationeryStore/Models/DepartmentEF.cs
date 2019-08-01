using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("Department", Schema = "dbo")]
    public class DepartmentEF
    {
        [Key]
        [MaxLength(50)]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public string DepartmentCode {get; set;}

        [MaxLength(255)]
        public string DepartmentName { get; set; }

        [MaxLength(255)]
        public string ContactName { get; set; }

        public int? AuthorityId { get; set; }
        [ForeignKey("AuthorityId")]
        public virtual StaffEF Authority { get; set; }

        public int? DepartmentRepresentativeId { get; set; }
        [ForeignKey("DepartmentRepresentativeId")]
        public virtual StaffEF DepartmentRepresentative { get; set; }

        public long? DelegationStartDate { get; set; }

        public long? DelegationEndDate { get; set; }

        [MaxLength(255)]
        public string TelephoneNo { get; set; }

        [MaxLength(255)]
        public string FaxNo { get; set; }

        public int? CollectionPointId { get; set; }
        [ForeignKey("CollectionPointId")]
        public virtual CollectionPointEF CollectionPoint { get; set; }

        [NotMapped]
        public virtual ICollection<StaffEF> Staff { get; set; }

        public virtual ICollection<StationeryDisbursementEF> StationeryDisbursements { get; set; }
        
    }
}