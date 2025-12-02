using ERP_API.DataAccess.Entities.Inventory;
using ERP_API.DataAccess.Entities.InventoryAdjustment;
using ERP_API.DataAccess.Entities.Warehouse;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.DataContext
{
    internal class ErpDBContext: DbContext
    {
        public ErpDBContext(
           DbContextOptions<ErpDBContext> options) : base(options) { }

        // Define your Tables
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariation> ProductVariations { get; set; }
        public DbSet<ProductPackage> ProductPackages { get; set; }
        public DbSet<PackageType> PackageTypes { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseStock> WarehouseStocks { get; set; }

        public DbSet<StockTransferLog> StockTransferLogs { get; set; }

        public DbSet<InventoryAdjustment> InventoryAdjustments { get; set; }

        // Optional: Categories if you have them
        //public DbSet<Category> Categories { get; set; }

        // ✅ ADD THIS METHOD
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configure Money (18 digits total, 2 after decimal) ---

            // ProductPackage Prices
            modelBuilder.Entity<ProductPackage>()
                .Property(p => p.PurchasePrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProductPackage>()
                .Property(p => p.SalesPrice)
                .HasColumnType("decimal(18,2)");

            // --- Configure Quantities (18 digits total, 4 after decimal) ---
            // We use 4 decimal places to be safe for weights (e.g., 0.005 KG)

            // ProductPackage QinP
            modelBuilder.Entity<ProductPackage>()
                .Property(p => p.QinP)
                .HasColumnType("decimal(18,4)");

            // WarehouseStock Quantities
            modelBuilder.Entity<WarehouseStock>()
                .Property(w => w.Quantity)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<WarehouseStock>()
                .Property(w => w.MinStockLevel)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<StockTransferLog>()
                .Property(l => l.Quantity)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<StockTransferLog>()
                .HasOne(l => l.FromWarehouse)
                .WithMany()
                .HasForeignKey(l => l.FromWarehouseId)
                .OnDelete(DeleteBehavior.Restrict); // <--- Crucial Change

            modelBuilder.Entity<StockTransferLog>()
                .HasOne(l => l.ToWarehouse)
                .WithMany()
                .HasForeignKey(l => l.ToWarehouseId)
                .OnDelete(DeleteBehavior.Restrict); // <--- Crucial Change

            modelBuilder.Entity<InventoryAdjustment>().Property(a => a.OldQuantity).HasColumnType("decimal(18,4)");
            modelBuilder.Entity<InventoryAdjustment>().Property(a => a.NewQuantity).HasColumnType("decimal(18,4)");
            modelBuilder.Entity<InventoryAdjustment>().Property(a => a.Difference).HasColumnType("decimal(18,4)");
        }
    }

    
}
