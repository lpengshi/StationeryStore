using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StationeryStore.Models
{
    [Table("Role", Schema = "dbo")]
    public class RoleEF
    {
        [Key]
        public int RoleId { get; set; }

        [MaxLength(50)]
        public string Description { get; set; }
    }
}