using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("RequestTemplate", Schema = "dbo")]
    public class RequestTemplateEF
    {
        [Key]
        public int TemplateId { get; set; }

        public int StaffId { get; set; }
        [ForeignKey("StaffId")]
        public virtual StaffEF Staff { get; set; }

    }
}