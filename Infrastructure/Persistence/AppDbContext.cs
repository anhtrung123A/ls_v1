using app.Domain.Constants;
using app.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using FileEntity = app.Domain.Entities.File;

namespace app.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<FileEntity> Files => Set<FileEntity>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Branch> Branches => Set<Branch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.HasQueryFilter(e => e.DeletedAt == null);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Firstname)
                .HasColumnName("firstname")
                .HasMaxLength(UserConstants.FirstnameMaxLength);

            entity.Property(e => e.Lastname)
                .HasColumnName("lastname")
                .HasMaxLength(UserConstants.LastnameMaxLength)
                .IsRequired();

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(UserConstants.EmailMaxLength)
                .IsRequired();

            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("ux_users_email");

            entity.Property(e => e.Phonenumber)
                .HasColumnName("phonenumber")
                .HasMaxLength(UserConstants.PhoneNumberMaxLength);

            entity.Property(e => e.DateOfBirth)
                .HasColumnName("dateofbirth");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime(6)")
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("datetime(6)")
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.DeletedAt)
                .HasColumnName("deleted_at")
                .HasColumnType("datetime(6)");

            entity.Property(e => e.AvatarFileId)
                .HasColumnName("avatar_file_id");

            entity.HasIndex(e => e.AvatarFileId)
                .HasDatabaseName("ix_users_avatar_file_id");

            entity.HasOne<FileEntity>()
                .WithMany()
                .HasForeignKey(e => e.AvatarFileId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<FileEntity>(entity =>
        {
            entity.ToTable("files");
            entity.HasKey(e => e.Id);
            entity.HasQueryFilter(e => e.DeletedAt == null);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.ObjectKey)
                .HasColumnName("object_key")
                .IsRequired();

            entity.Property(e => e.BucketName)
                .HasColumnName("bucket_name");

            entity.Property(e => e.FileName)
                .HasColumnName("file_name");

            entity.Property(e => e.ContentType)
                .HasColumnName("content_type");

            entity.Property(e => e.Size)
                .HasColumnName("size");

            entity.Property(e => e.Checksum)
                .HasColumnName("checksum");

            entity.Property(e => e.IsPublic)
                .HasColumnName("is_public")
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime(6)");

            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by");

            entity.Property(e => e.DeletedAt)
                .HasColumnName("deleted_at")
                .HasColumnType("datetime(6)");

            entity.Property(e => e.Metadata)
                .HasColumnName("metadata")
                .HasColumnType("json");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(e => e.Id);
            entity.HasQueryFilter(e => e.DeletedAt == null);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime(6)")
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("datetime(6)")
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.CreatedByUserId)
                .HasColumnName("created_by_user_id");

            entity.Property(e => e.UpdatedByUserId)
                .HasColumnName("updated_by_user_id");

            entity.Property(e => e.DeletedAt)
                .HasColumnName("deleted_at")
                .HasColumnType("datetime(6)");

            entity.HasIndex(e => e.Name)
                .IsUnique()
                .HasDatabaseName("ux_roles_name");

            entity.HasData(
                new Role { Id = 1, Name = "admin", CreatedAt = new DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc) },
                new Role { Id = 2, Name = "branch_manager", CreatedAt = new DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc) },
                new Role { Id = 3, Name = "operator", CreatedAt = new DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc) },
                new Role { Id = 4, Name = "teacher", CreatedAt = new DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc) },
                new Role { Id = 5, Name = "student", CreatedAt = new DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc) },
                new Role { Id = 6, Name = "parent", CreatedAt = new DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc) }
            );
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("user_roles");
            entity.HasKey(e => new { e.UserId, e.RoleId });
            entity.HasQueryFilter(e => e.DeletedAt == null);

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");

            entity.Property(e => e.RoleId)
                .HasColumnName("role_id");

            entity.Property(e => e.AssignedAt)
                .HasColumnName("assigned_at")
                .HasColumnType("datetime(6)")
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime(6)")
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("datetime(6)")
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.CreatedByUserId)
                .HasColumnName("created_by_user_id");

            entity.Property(e => e.UpdatedByUserId)
                .HasColumnName("updated_by_user_id");

            entity.Property(e => e.DeletedAt)
                .HasColumnName("deleted_at")
                .HasColumnType("datetime(6)");

            entity.HasIndex(e => e.RoleId)
                .HasDatabaseName("ix_user_roles_role_id");

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Role>()
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.ToTable("branches");
            entity.HasKey(e => e.Id);
            entity.HasQueryFilter(e => e.DeletedAt == null);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(BranchConstants.NameMaxLength)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(BranchConstants.DescriptionMaxLength);

            entity.Property(e => e.AddressLine1)
                .HasColumnName("address_line1")
                .HasMaxLength(BranchConstants.AddressLineMaxLength);

            entity.Property(e => e.AddressLine2)
                .HasColumnName("address_line2")
                .HasMaxLength(BranchConstants.AddressLineMaxLength);

            entity.Property(e => e.Ward)
                .HasColumnName("ward")
                .HasMaxLength(BranchConstants.WardMaxLength);

            entity.Property(e => e.District)
                .HasColumnName("district")
                .HasMaxLength(BranchConstants.DistrictMaxLength);

            entity.Property(e => e.City)
                .HasColumnName("city")
                .HasMaxLength(BranchConstants.CityMaxLength);

            entity.Property(e => e.PostalCode)
                .HasColumnName("postal_code")
                .HasMaxLength(BranchConstants.PostalCodeMaxLength);

            entity.Property(e => e.Country)
                .HasColumnName("country")
                .HasMaxLength(BranchConstants.CountryMaxLength);

            entity.Property(e => e.ImageFileId)
                .HasColumnName("image_file_id");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime(6)")
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("datetime(6)")
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.CreatedByUserId)
                .HasColumnName("created_by_user_id");

            entity.Property(e => e.UpdatedByUserId)
                .HasColumnName("updated_by_user_id");

            entity.Property(e => e.DeletedAt)
                .HasColumnName("deleted_at")
                .HasColumnType("datetime(6)");

            entity.HasIndex(e => e.ImageFileId)
                .HasDatabaseName("ix_branches_image_file_id");

            entity.HasOne<FileEntity>()
                .WithMany()
                .HasForeignKey(e => e.ImageFileId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
