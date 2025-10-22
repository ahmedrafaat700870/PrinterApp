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
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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
            entity.Property(e => e.KnifeFactor).IsRequired().HasColumnType("decimal(18,4)");
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
            entity.Property(e => e.CartonFactor).IsRequired().HasColumnType("decimal(18,4)");
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
            entity.Property(e => e.MaxWidth).IsRequired().HasColumnType("decimal(18,4)");
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
            entity.Property(e => e.CoreCoefficient).IsRequired().HasColumnType("decimal(18,4)");
            entity.Property(e => e.WidthCor).IsRequired().HasColumnType("decimal(18,4)");
            entity.Property(e => e.HeightCor).IsRequired().HasColumnType("decimal(18,4)");
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