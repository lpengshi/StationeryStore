﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StationeryStore.ERD
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AdjustmentVoucher> AdjustmentVoucher { get; set; }
        public virtual DbSet<AdjustmentVoucherDetails> AdjustmentVoucherDetails { get; set; }
        public virtual DbSet<CatalogueItem> CatalogueItem { get; set; }
        public virtual DbSet<CollectionPoint> CollectionPoint { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public virtual DbSet<PurchaseOrderDetails> PurchaseOrderDetails { get; set; }
        public virtual DbSet<RequestTemplate> RequestTemplate { get; set; }
        public virtual DbSet<RequestTemplateDetails> RequestTemplateDetails { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<StationeryDisbursement> StationeryDisbursement { get; set; }
        public virtual DbSet<StationeryDisbursementDetails> StationeryDisbursementDetails { get; set; }
        public virtual DbSet<StationeryRequest> StationeryRequest { get; set; }
        public virtual DbSet<StationeryRequestDetails> StationeryRequestDetails { get; set; }
        public virtual DbSet<StationeryRetrieval> StationeryRetrieval { get; set; }
        public virtual DbSet<Stock> Stock { get; set; }
        public virtual DbSet<StockTransactionDetails> StockTransactionDetails { get; set; }
        public virtual DbSet<Supplier> Supplier { get; set; }
        public virtual DbSet<SupplierDetails> SupplierDetails { get; set; }
    }
}
