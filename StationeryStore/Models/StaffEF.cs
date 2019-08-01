using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
    [Table("Staff", Schema = "dbo")]
    public class StaffEF
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public int StaffId { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [Index("INDEX_USERNAME", IsClustered = false, IsUnique = true)]
        [MaxLength(50)]
        public string Username { get; set; }

        [MaxLength(50)]
        public string Password { get; set; }

        public string DepartmentCode { get; set; }
        [ForeignKey("DepartmentCode")]
        public virtual DepartmentEF Department { get; set; }

        public string Email { get; set; }

        [MaxLength(50)]
        public string SessionId { get; set; }

        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual RoleEF Role { get; set; }
    }
}
