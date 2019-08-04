using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.Models
{
	public class ManageCollectionDTO
	{
        public string Department { get; set; }

        public List<CollectionPointEF> CollectionPoints { get; set; }

        public int CollectionPointId { get; set; }

        public int? DepartmentRepId { get; set; }
	}
}