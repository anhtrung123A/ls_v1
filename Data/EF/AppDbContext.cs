using Microsoft.EntityFrameworkCore;
using app.Data.EF.Entities;

namespace app.Data.EF;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(u => u.FullName)
                .HasColumnName("full_name")
                .HasMaxLength(150);

            entity.Property(u => u.Email)
                .HasColumnName("email")
                .HasMaxLength(150)
                .IsRequired();

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.Phone)
                .HasColumnName("phone")
                .HasMaxLength(20);

            entity.Property(u => u.Role)
                .HasColumnName("role")
                .HasColumnType("tinyint");

            entity.Property(u => u.AvatarUrl)
                .HasColumnName("avatar_url")
                .HasColumnType("text");

            entity.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .HasColumnType("text");

            entity.Property(u => u.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            entity.Property(u => u.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(u => u.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.ToTable("staff");

            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(s => s.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(s => s.StaffCode)
                .HasColumnName("staff_code")
                .HasMaxLength(20);

            entity.HasIndex(s => s.StaffCode)
                .IsUnique();

            entity.Property(s => s.Department)
                .HasColumnName("department")
                .HasColumnType("tinyint");

            entity.Property(s => s.Position)
                .HasColumnName("position")
                .HasMaxLength(100);

            entity.Property(s => s.ManagerId)
                .HasColumnName("manager_id");

            entity.Property(s => s.JoinedAt)
                .HasColumnName("joined_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Staff>()
                .WithMany()
                .HasForeignKey(s => s.ManagerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("students");

            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(s => s.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(s => s.StudentCode)
                .HasColumnName("student_code")
                .HasMaxLength(20);

            entity.HasIndex(s => s.StudentCode)
                .IsUnique();

            entity.Property(s => s.DateOfBirth)
                .HasColumnName("date_of_birth")
                .HasColumnType("date");

            entity.Property(s => s.Gender)
                .HasColumnName("gender")
                .HasColumnType("tinyint");

            entity.Property(s => s.Address)
                .HasColumnName("address")
                .HasColumnType("text");

            entity.Property(s => s.ParentName)
                .HasColumnName("parent_name")
                .HasMaxLength(150);

            entity.Property(s => s.ParentPhone)
                .HasColumnName("parent_phone")
                .HasMaxLength(20);

            entity.Property(s => s.ParentEmail)
                .HasColumnName("parent_email")
                .HasMaxLength(150);

            entity.Property(s => s.Source)
                .HasColumnName("source")
                .HasColumnType("tinyint");

            entity.Property(s => s.AssignedStaffId)
                .HasColumnName("assigned_staff_id");

            entity.Property(s => s.Status)
                .HasColumnName("status")
                .HasColumnType("tinyint")
                .HasDefaultValue((byte)1);

            entity.Property(s => s.EnrolledAt)
                .HasColumnName("enrolled_at")
                .HasColumnType("timestamp");

            entity.Property(s => s.Notes)
                .HasColumnName("notes")
                .HasColumnType("text");

            entity.Property(s => s.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Staff>()
                .WithMany()
                .HasForeignKey(s => s.AssignedStaffId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        base.OnModelCreating(modelBuilder);
    }
}
