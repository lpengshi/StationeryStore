//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StationeryStore
{
    using System;
    using System.Collections.Generic;
    
    public partial class CatalogueItem
    {
        public int CatalogueId { get; set; }
        public string ItemCode { get; set; }
        public int ReorderLevel { get; set; }
        public int ReorderQty { get; set; }
    
        public virtual Stock Stock { get; set; }
    }
}
