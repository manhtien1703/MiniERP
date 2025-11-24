using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class MiniErpDbContext : DbContext
{
    public MiniErpDbContext(DbContextOptions<MiniErpDbContext> options) : base(options) { }

    public DbSet<Province> Provinces { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<CoolingDevice> Devices { get; set; }
    public DbSet<DeviceLog> DeviceLogs { get; set; }
    public DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Warehouse>()
            .HasOne(w => w.Province)
            .WithMany(p => p.Warehouses)
            .HasForeignKey(w => w.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cấu hình rõ ràng kiểu dữ liệu cho CoolingDevice
        modelBuilder.Entity<CoolingDevice>(entity =>
        {
            entity.Property(e => e.Id)
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8mb4");
            
            entity.Property(e => e.WarehouseId)
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8mb4");
        });

        // Cấu hình rõ ràng kiểu dữ liệu cho DeviceLog
        modelBuilder.Entity<DeviceLog>(entity =>
        {
            entity.Property(e => e.DeviceId)
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8mb4");
            
            // Đảm bảo Timestamp được lưu và đọc như UTC
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime(6)")
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                );
        });

        // Cấu hình cho User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100);
        });

        // Seed test users (password: "123456" for all)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                FullName = "Administrator",
                Email = "admin@minierp.com",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new User
            {
                Id = 2,
                Username = "user1",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                FullName = "Nguyễn Văn A",
                Email = "user1@minierp.com",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new User
            {
                Id = 3,
                Username = "user2",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                FullName = "Trần Thị B",
                Email = "user2@minierp.com",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        );

        modelBuilder.Entity<Province>().HasData(
            new Province { Id = 1, Code = "TQ", Name = "Tuyên Quang" },
            new Province { Id = 2, Code = "LC", Name = "Lào Cai" },
            new Province { Id = 3, Code = "TNG", Name = "Thái Nguyên" },
            new Province { Id = 4, Code = "PT", Name = "Phú Thọ" },
            new Province { Id = 5, Code = "BN", Name = "Bắc Ninh" },
            new Province { Id = 6, Code = "HY", Name = "Hưng Yên" },
            new Province { Id = 7, Code = "HP", Name = "Hải Phòng" },
            new Province { Id = 8, Code = "NB", Name = "Ninh Bình" },
            new Province { Id = 9, Code = "QT", Name = "Quảng Trị" },
            new Province { Id = 10, Code = "DAN", Name = "Đà Nẵng" },
            new Province { Id = 11, Code = "QNG", Name = "Quảng Ngãi" },
            new Province { Id = 12, Code = "GL", Name = "Gia Lai" },
            new Province { Id = 13, Code = "KH", Name = "Khánh Hòa" },
            new Province { Id = 14, Code = "LD", Name = "Lâm Đồng" },
            new Province { Id = 15, Code = "DLK", Name = "Đắk Lắk" },
            new Province { Id = 16, Code = "HCM", Name = "Hồ Chí Minh" },
            new Province { Id = 17, Code = "DNI", Name = "Đồng Nai" },
            new Province { Id = 18, Code = "TNI", Name = "Tây Ninh" },
            new Province { Id = 19, Code = "CT", Name = "Cần Thơ" },
            new Province { Id = 20, Code = "VL", Name = "Vĩnh Long" },
            new Province { Id = 21, Code = "DTP", Name = "Đồng Tháp" },
            new Province { Id = 22, Code = "CM", Name = "Cà Mau" },
            new Province { Id = 23, Code = "AG", Name = "An Giang" },
            new Province { Id = 24, Code = "HN", Name = "Hà Nội" },
            new Province { Id = 25, Code = "HUE", Name = "Huế" },
            new Province { Id = 26, Code = "LCH", Name = "Lai Châu" },
            new Province { Id = 27, Code = "DB", Name = "Điện Biên" },
            new Province { Id = 28, Code = "SL", Name = "Sơn La" },
            new Province { Id = 29, Code = "LS", Name = "Lạng Sơn" },
            new Province { Id = 30, Code = "QNI", Name = "Quảng Ninh" },
            new Province { Id = 31, Code = "TH", Name = "Thanh Hóa" },
            new Province { Id = 32, Code = "NA", Name = "Nghệ An" },
            new Province { Id = 33, Code = "HT", Name = "Hà Tĩnh" },
            new Province { Id = 34, Code = "CB", Name = "Cao Bằng" }
        );
    }
}
