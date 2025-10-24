using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Permission> Permissions { get; set; }
    public DbSet<PermissionRole> PermissionRoles { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }
    public DbSet<Core> Cores { get; set; }
    public DbSet<RollDirection> RollDirections { get; set; }
    public DbSet<Machine> Machines { get; set; }
    public DbSet<Carton> Cartons { get; set; }
    public DbSet<Knife> Knives { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<RawMaterial> RawMaterials { get; set; }
    public DbSet<ManufacturingAddition> ManufacturingAdditions { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductAddition> ProductAdditions { get; set; }
    public DbSet<MoldShape> MoldShapes { get; set; }
    public DbSet<Mold> Molds { get; set; }




    // DbSets الجديدة
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderAttachment> OrderAttachments { get; set; }
    public DbSet<OrderManufacturingItem> OrderManufacturingItems { get; set; }
    public DbSet<OrderTimeline> OrderTimelines { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);



        // ===== Customer Configuration =====
        builder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CustomerCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(200);
            entity.Property(e => e.Governorate).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.CreatedBy).HasMaxLength(450);
            entity.Property(e => e.ModifiedBy).HasMaxLength(450).IsRequired(false);
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.CreatedDate).IsRequired();

            // الفهارس
            entity.HasIndex(e => e.CustomerCode).IsUnique();
            entity.HasIndex(e => e.CustomerName);
            entity.HasIndex(e => e.Phone);
        });

        // ===== OrderAttachment Configuration =====
        builder.Entity<OrderAttachment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.OriginalFileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FileExtension).HasMaxLength(50);
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.UploadedBy).HasMaxLength(450);
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.UploadedDate).IsRequired();

            // العلاقة
            entity.HasOne(e => e.Order)
                  .WithMany(o => o.Attachments)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            // الفهارس
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.UploadedDate);
        });

        // ===== OrderManufacturingItem Configuration =====
        builder.Entity<OrderManufacturingItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.CompletedBy).HasMaxLength(450);
            entity.Property(e => e.IsCompleted).IsRequired().HasDefaultValue(false);

            // العلاقات
            entity.HasOne(e => e.Order)
                  .WithMany(o => o.ManufacturingItems)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ManufacturingAddition)
                  .WithMany()
                  .HasForeignKey(e => e.ManufacturingAdditionId)
                  .OnDelete(DeleteBehavior.Restrict);

            // الفهارس
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.IsCompleted);
            entity.HasIndex(e => e.DisplayOrder);
        });

        // ===== OrderTimeline Configuration =====
        builder.Entity<OrderTimeline>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Action).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(1000).IsRequired(false);
            entity.Property(e => e.ActionBy).HasMaxLength(450);
            entity.Property(e => e.ActionByName).HasMaxLength(100);
            entity.Property(e => e.ActionDate).IsRequired();

            // العلاقة
            entity.HasOne(e => e.Order)
                  .WithMany(o => o.Timeline)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            // الفهارس
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.ActionDate);
            entity.HasIndex(e => e.Stage);
        });

        // ===== Order Configuration =====
        builder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.OrderDate).IsRequired();
            entity.Property(e => e.ExpectedDeliveryDate).IsRequired();
            entity.Property(e => e.Length).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Width).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.OrderNotes).HasMaxLength(2000).IsRequired(false);
            entity.Property(e => e.ReviewNotes).HasMaxLength(2000).IsRequired(false);
            entity.Property(e => e.ManufacturingNotes).HasMaxLength(2000).IsRequired(false);
            entity.Property(e => e.PrintingNotes).HasMaxLength(2000);
            entity.Property(e => e.CreatedBy).HasMaxLength(450).IsRequired(false);
            entity.Property(e => e.ModifiedBy).HasMaxLength(450).IsRequired(false);
            entity.Property(e => e.ReviewedBy).HasMaxLength(450).IsRequired(false);
            entity.Property(e => e.PrintedBy).HasMaxLength(450).IsRequired();
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

            // العلاقات
            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Orders)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Supplier)
                  .WithMany()
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.RollDirection)
                  .WithMany()
                  .HasForeignKey(e => e.RollDirectionId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.RawMaterial)
                  .WithMany()
                  .HasForeignKey(e => e.RawMaterialId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Machine)
                  .WithMany()
                  .HasForeignKey(e => e.MachineId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Core)
                  .WithMany()
                  .HasForeignKey(e => e.CoreId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Knife)
                  .WithMany()
                  .HasForeignKey(e => e.KnifeId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Carton)
                  .WithMany()
                  .HasForeignKey(e => e.CartonId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Mold)
                  .WithMany()
                  .HasForeignKey(e => e.MoldId)
                  .OnDelete(DeleteBehavior.SetNull);

            // الفهارس
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.HasIndex(e => e.OrderDate);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Stage);
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.CreatedDate);
        });






        // Configure MoldShape
        builder.Entity<MoldShape>(entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.ShapeName).IsRequired().HasMaxLength(100);
                    entity.Property(e => e.ShapeImagePath).HasMaxLength(500);
                    entity.Property(e => e.Description).HasMaxLength(500);
                    entity.Property(e => e.CreatedDate).IsRequired();
                    entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
                    entity.HasIndex(e => e.ShapeName).IsUnique();
                });

        // Configure Mold
        builder.Entity<Mold>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MoldNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Width).IsRequired();
            entity.Property(e => e.Height).IsRequired();
            entity.Property(e => e.TotalEyes).IsRequired();
            entity.Property(e => e.PrintedRawMaterialSize).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.PlainRawMaterialSize).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

            // Foreign Keys
            entity.HasOne(e => e.Machine)
                  .WithMany()
                  .HasForeignKey(e => e.MachineId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.MoldShape)
                  .WithMany(ms => ms.Molds)
                  .HasForeignKey(e => e.MoldShapeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.MoldNumber).IsUnique();
        });


        // Configure Product
        builder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ProductCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.IsPrinted).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

            // Foreign Keys
            entity.HasOne(e => e.RawMaterial)
                  .WithMany()
                  .HasForeignKey(e => e.RawMaterialId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Supplier)
                  .WithMany()
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.ProductCode).IsUnique();
            entity.HasIndex(e => e.ProductName);
        });

        // Configure ProductAddition (Many-to-Many)
        builder.Entity<ProductAddition>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Product)
                  .WithMany(p => p.ProductAdditions)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ManufacturingAddition)
                  .WithMany()
                  .HasForeignKey(e => e.ManufacturingAdditionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.ProductId, e.ManufacturingAdditionId }).IsUnique();
        });




        // Configure ManufacturingAddition
        builder.Entity<ManufacturingAddition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AdditionName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.HasIndex(e => e.AdditionName).IsUnique();
        });
        // Configure RawMaterial
        builder.Entity<RawMaterial>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RawMaterialName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Width).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Height).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.AreaSquareMeters).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PricePerSquareMeter).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PricePerLinearMeter).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.HasIndex(e => e.RawMaterialName).IsUnique();
        });
        // Configure Supplier
        builder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SupplierCode).IsRequired().HasMaxLength(4);
            entity.Property(e => e.SupplierName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CardNumber).HasMaxLength(50);
            entity.Property(e => e.CommercialRegister).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.HasIndex(e => e.SupplierCode).IsUnique();
            entity.HasIndex(e => e.SupplierName).IsUnique();
        });

        // Configure Knife
        builder.Entity<Knife>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.KnifeName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.KnifeFactor).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.HasIndex(e => e.KnifeName).IsUnique();
        });
        // Configure Carton
        builder.Entity<Carton>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CartonName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CartonFactor).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.HasIndex(e => e.CartonName).IsUnique();
        });
        // Configure Machine
        builder.Entity<Machine>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MachineName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.MaxWidth).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ModelNumber).HasMaxLength(100);
            entity.Property(e => e.Manufacturer).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.HasIndex(e => e.MachineName).IsUnique();
        });

        // Configure RollDirection
        builder.Entity<RollDirection>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DirectionNumber).IsRequired();
            entity.Property(e => e.DirectionImage).HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.HasIndex(e => e.DirectionNumber).IsUnique();
        });

        // Configure Core
        builder.Entity<Core>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CoreName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CoreCoefficient).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.WidthCor).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.HeightCor).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.HasIndex(e => e.CoreName).IsUnique();
        });
        // Configure SystemSetting
        builder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Value).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.HasIndex(e => e.Key).IsUnique();
        });
        // Configure Permission
        builder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Configure PermissionRole
        builder.Entity<PermissionRole>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);

            entity.HasOne(e => e.Permission)
                .WithMany(p => p.PermissionRoles)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.PermissionId, e.RoleName }).IsUnique();
        });

        // Configure UserPermission
        builder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserPermissions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Permission)
                .WithMany(p => p.UserPermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PermissionRole)
                .WithMany(pr => pr.UserPermissions)
                .HasForeignKey(e => e.PermissionRoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.UserId, e.PermissionId, e.PermissionRoleId }).IsUnique();
        });
    }
}