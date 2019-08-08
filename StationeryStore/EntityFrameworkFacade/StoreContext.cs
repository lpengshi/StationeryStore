namespace StationeryStore.EntityFrameworkFacade
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using StationeryStore.Models;

    public class StoreContext : DbContext
    {
        public StoreContext() : base("name=StoreConnectionString")
        {
            //Database.SetInitializer<StoreContext>(new CreateDatabaseIfNotExists<StoreContext>());
            Database.SetInitializer<StoreContext>(new DropCreateDatabaseIfModelChanges<StoreContext>());
            //Database.SetInitializer<StoreContext>(new DropCreateDatabaseAlways<StoreContext>());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }

        public virtual DbSet<AdjustmentVoucherEF> AdjustmentVouchers { get; set; }
        public virtual DbSet<AdjustmentVoucherDetailsEF> AdjustmentVouchersDetails { get; set; }
        public virtual DbSet<CatalogueItemEF> CatalogueItems { get; set; }
        public virtual DbSet<CollectionPointEF> CollectionPoints { get; set; }
        public virtual DbSet<DepartmentEF> Departments { get; set; }
        public virtual DbSet<PurchaseOrderEF> PurchaseOrders { get; set; }
        public virtual DbSet<PurchaseOrderDetailsEF> PurchaseOrderDetails { get; set; }
        public virtual DbSet<RoleEF> Roles { get; set; }
        public virtual DbSet<StaffEF> Staff { get; set; }
        public virtual DbSet<StationeryRequestEF> StationeryRequests { get; set; }
        public virtual DbSet<StationeryRequestDetailsEF> StationeryRequestDetails { get; set; }
        public virtual DbSet<StationeryRetrievalEF> StationeryRetrievals { get; set; }
        public virtual DbSet<StockEF> Stocks { get; set; }
        public virtual DbSet<StockTransactionDetailsEF> StockTransactionDetails { get; set; }
        public virtual DbSet<SupplierEF> Suppliers { get; set; }
        public virtual DbSet<SupplierDetailsEF> SupplierDetails { get; set; }
        public virtual DbSet<StationeryDisbursementEF> StationeryDisbursements { get; set; }
        public virtual DbSet<StationeryDisbursementDetailsEF> StationeryDisbursementDetails { get; set; }
        public virtual DbSet<RequestTemplateEF> RequestTemplates { get; set; }
        public virtual DbSet<RequestTemplateDetailsEF> RequestTemplateDetails { get; set; }

    }
}