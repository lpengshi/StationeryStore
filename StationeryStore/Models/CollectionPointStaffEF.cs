using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("CollectionPointStaff", Schema = "dbo")]
    public class CollectionPointStaffEF
    {
        [Key]
        [Column(Order = 1)]
        public int CollectionPointId { get; set; }
        [ForeignKey("CollectionPointId")]
        public virtual CollectionPointEF CollectionPoint { get; set; }

        [Key]
        [Column(Order = 2)]
        public int StaffId { get; set; }
        [ForeignKey("StaffId")]
        public virtual StaffEF Staff { get; set; }

    }
}