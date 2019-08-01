using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("CollectionPoint", Schema = "dbo")]
    public class CollectionPointEF
    {
        [Key]
        public int CollectionPointId { get; set; }

        [MaxLength(50)]
        public string Location { get; set; }

        public string CollectionTime { get; set; }
    }
}