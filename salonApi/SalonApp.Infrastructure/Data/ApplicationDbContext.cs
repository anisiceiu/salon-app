using Microsoft.EntityFrameworkCore;
using SalonApp.Domain.Entities;

namespace SalonApp.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<StaffService> StaffServices => Set<StaffService>();
    public DbSet<WorkingHours> WorkingHours => Set<WorkingHours>();
    public DbSet<Leave> Leaves => Set<Leave>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AppointmentService> AppointmentServices => Set<AppointmentService>();
    public DbSet<Discount> Discounts => Set<Discount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Role).IsRequired();
        });

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired();
            entity.Property(e => e.CreatedByIp).IsRequired().HasMaxLength(50);
            
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ServiceCategory configuration
        modelBuilder.Entity<ServiceCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasMany(e => e.Services)
                .WithOne(s => s.Category)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Service configuration
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Duration).IsRequired();
            entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Staff configuration (extends User)
        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.ProfileImage).HasMaxLength(255);

            entity.HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Staff>(e => e.Id)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // StaffService (Many-to-Many)
        modelBuilder.Entity<StaffService>(entity =>
        {
            entity.HasKey(e => new { e.StaffId, e.ServiceId });

            entity.HasOne(e => e.Staff)
                .WithMany(s => s.StaffServices)
                .HasForeignKey(e => e.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // WorkingHours configuration
        modelBuilder.Entity<WorkingHours>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Staff)
                .WithMany(s => s.WorkingHours)
                .HasForeignKey(e => e.StaffId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Leave configuration
        modelBuilder.Entity<Leave>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired();

            entity.HasOne(e => e.Staff)
                .WithMany(s => s.Leaves)
                .HasForeignKey(e => e.StaffId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Appointment configuration
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.FinalPrice).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).IsRequired();

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Staff)
                .WithMany()
                .HasForeignKey(e => e.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Discount)
                .WithMany()
                .HasForeignKey(e => e.DiscountId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // AppointmentService configuration (Many-to-Many between Appointment and Service)
        modelBuilder.Entity<AppointmentService>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Appointment)
                .WithMany()
                .HasForeignKey(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Discount configuration
        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DiscountType).IsRequired();
            entity.Property(e => e.DiscountValue).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.MinOrderValue).HasColumnType("decimal(18,2)");
        });
    }
}
