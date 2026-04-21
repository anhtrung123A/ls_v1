using app.Domain.Constants;
using app.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace app.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);

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
        });
    }
}
