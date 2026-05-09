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
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<BatchJobExecution> BatchJobExecutions => Set<BatchJobExecution>();
    public DbSet<BatchJobItem> BatchJobItems => Set<BatchJobItem>();
    public DbSet<BatchJobLock> BatchJobLocks => Set<BatchJobLock>();
    public DbSet<Interaction> Interactions => Set<Interaction>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

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
                .HasColumnName("user_id");

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

        modelBuilder.Entity<Lead>(entity =>
        {
            entity.ToTable("leads");

            entity.HasKey(l => l.Id);

            entity.Property(l => l.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(l => l.FullName)
                .HasColumnName("full_name")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(l => l.Phone)
                .HasColumnName("phone")
                .HasMaxLength(20);

            entity.Property(l => l.Email)
                .HasColumnName("email")
                .HasMaxLength(150);

            entity.Property(l => l.Source)
                .HasColumnName("source")
                .HasColumnType("tinyint");

            entity.Property(l => l.Campaign)
                .HasColumnName("campaign")
                .HasMaxLength(200);

            entity.Property(l => l.Interest)
                .HasColumnName("interest")
                .HasColumnType("text");

            entity.Property(l => l.Status)
                .HasColumnName("status")
                .HasColumnType("tinyint")
                .HasDefaultValue((byte)1);

            entity.Property(l => l.LostReason)
                .HasColumnName("lost_reason")
                .HasColumnType("text");

            entity.Property(l => l.AssignedTo)
                .HasColumnName("assigned_to");

            entity.Property(l => l.ConvertedTo)
                .HasColumnName("converted_to");

            entity.Property(l => l.Note)
                .HasColumnName("note")
                .HasColumnType("text");

            entity.Property(l => l.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(l => l.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(l => l.ConvertedAt)
                .HasColumnName("converted_at")
                .HasColumnType("timestamp");

            entity.HasOne<Staff>()
                .WithMany()
                .HasForeignKey(l => l.AssignedTo)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne<Student>()
                .WithMany()
                .HasForeignKey(l => l.ConvertedTo)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<BatchJobExecution>(entity =>
        {
            entity.ToTable("batch_job_executions");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.JobName)
                .HasColumnName("job_name")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasColumnName("status")
                .HasColumnType("tinyint")
                .IsRequired();

            entity.Property(x => x.TriggeredBy)
                .HasColumnName("triggered_by")
                .HasColumnType("tinyint");

            entity.Property(x => x.StartedAt)
                .HasColumnName("started_at")
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(x => x.FinishedAt)
                .HasColumnName("finished_at")
                .HasColumnType("datetime");

            entity.Property(x => x.DurationMs)
                .HasColumnName("duration_ms");

            entity.Property(x => x.ErrorMessage)
                .HasColumnName("error_message")
                .HasColumnType("text");

            entity.Property(x => x.ErrorTrace)
                .HasColumnName("error_trace")
                .HasColumnType("text");

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("datetime")
                .IsRequired();

            entity.HasIndex(x => x.JobName);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.StartedAt);
        });

        modelBuilder.Entity<BatchJobItem>(entity =>
        {
            entity.ToTable("batch_job_items");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.ExecutionId)
                .HasColumnName("execution_id")
                .IsRequired();

            entity.Property(x => x.TargetType)
                .HasColumnName("target_type")
                .HasMaxLength(100);

            entity.Property(x => x.TargetId)
                .HasColumnName("target_id");

            entity.Property(x => x.Status)
                .HasColumnName("status")
                .HasColumnType("tinyint")
                .IsRequired();

            entity.Property(x => x.ErrorMessage)
                .HasColumnName("error_message")
                .HasColumnType("text");

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("datetime")
                .IsRequired();

            entity.HasIndex(x => x.ExecutionId);
            entity.HasIndex(x => new { x.TargetType, x.TargetId });
            entity.HasIndex(x => x.Status);

            entity.HasOne<BatchJobExecution>()
                .WithMany()
                .HasForeignKey(x => x.ExecutionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BatchJobLock>(entity =>
        {
            entity.ToTable("batch_job_locks");

            entity.HasKey(x => x.JobName);

            entity.Property(x => x.JobName)
                .HasColumnName("job_name")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.LockedAt)
                .HasColumnName("locked_at")
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(x => x.LockedBy)
                .HasColumnName("locked_by")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.ExpiresAt)
                .HasColumnName("expires_at")
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("datetime")
                .IsRequired();

            entity.HasIndex(x => x.ExpiresAt);
        });

        modelBuilder.Entity<Interaction>(entity =>
        {
            entity.ToTable("interactions");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.LeadId)
                .HasColumnName("lead_id");

            entity.Property(x => x.StaffId)
                .HasColumnName("staff_id");

            entity.Property(x => x.Channel)
                .HasColumnName("channel")
                .HasMaxLength(50);

            entity.Property(x => x.Direction)
                .HasColumnName("direction")
                .HasMaxLength(10);

            entity.Property(x => x.Content)
                .HasColumnName("content")
                .HasColumnType("text");

            entity.Property(x => x.Outcome)
                .HasColumnName("outcome")
                .HasMaxLength(100);

            entity.Property(x => x.Attachments)
                .HasColumnName("attachments")
                .HasColumnType("text");

            entity.Property(x => x.OccurredAt)
                .HasColumnName("occurred_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne<Lead>()
                .WithMany()
                .HasForeignKey(x => x.LeadId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne<Staff>()
                .WithMany()
                .HasForeignKey(x => x.StaffId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.AssignedTo)
                .HasColumnName("assigned_to");

            entity.Property(x => x.CreatedBy)
                .HasColumnName("created_by");

            entity.Property(x => x.RelatedLeadId)
                .HasColumnName("related_lead_id");

            entity.Property(x => x.Title)
                .HasColumnName("title")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasColumnName("description")
                .HasColumnType("text");

            entity.Property(x => x.Type)
                .HasColumnName("type")
                .HasMaxLength(50);

            entity.Property(x => x.Priority)
                .HasColumnName("priority")
                .HasColumnType("tinyint")
                .HasDefaultValue((byte)2);

            entity.Property(x => x.Status)
                .HasColumnName("status")
                .HasColumnType("tinyint")
                .HasDefaultValue((byte)1);

            entity.Property(x => x.DueAt)
                .HasColumnName("due_at")
                .HasColumnType("timestamp");

            entity.Property(x => x.DoneAt)
                .HasColumnName("done_at")
                .HasColumnType("timestamp");

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne<Staff>()
                .WithMany()
                .HasForeignKey(x => x.AssignedTo)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne<Staff>()
                .WithMany()
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne<Lead>()
                .WithMany()
                .HasForeignKey(x => x.RelatedLeadId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        base.OnModelCreating(modelBuilder);
    }
}
