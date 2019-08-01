using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("StationeryRetrieval", Schema = "dbo")]
    public class StationeryRetrievalEF
    {
        [Key]
        public int RetrievalId { get; set; }

        public long DateRetrieved { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        public virtual ICollection<StationeryDisbursementEF> StationeryDisbursements { get; set; }
    }
}
