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
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseCategory> CourseCategories => Set<CourseCategory>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<ClassEntity> Classes => Set<ClassEntity>();
    public DbSet<ClassSchedule> ClassSchedules => Set<ClassSchedule>();
    public DbSet<ClassSession> ClassSessions => Set<ClassSession>();
    public DbSet<ClassAttendance> ClassAttendances => Set<ClassAttendance>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<ClassStudent> ClassStudents => Set<ClassStudent>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<SalaryConfig> SalaryConfigs => Set<SalaryConfig>();
    public DbSet<Payroll> Payrolls => Set<Payroll>();
    public DbSet<PayrollItem> PayrollItems => Set<PayrollItem>();
    public DbSet<StaffKpiRecord> StaffKpiRecords => Set<StaffKpiRecord>();

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

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("courses");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasColumnName("description").HasColumnType("text");
            entity.Property(x => x.CategoryId).HasColumnName("category_id");
            entity.Property(x => x.Level).HasColumnName("level").HasMaxLength(50);
            entity.Property(x => x.TotalSessions).HasColumnName("total_sessions");
            entity.Property(x => x.DurationMinutes).HasColumnName("duration_minutes");
            entity.Property(x => x.Price).HasColumnName("price").HasPrecision(12, 2);
            entity.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(5).HasDefaultValue("VND");
            entity.Property(x => x.ThumbnailUrl).HasColumnName("thumbnail_url").HasColumnType("text");
            entity.Property(x => x.Status).HasColumnName("status").HasColumnType("tinyint").HasDefaultValue((byte)1);
            entity.Property(x => x.CreatedBy).HasColumnName("created_by");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne<CourseCategory>().WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<CourseCategory>(entity =>
        {
            entity.ToTable("course_categories");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(100);
            entity.Property(x => x.Description).HasColumnName("description").HasColumnType("text");
            entity.Property(x => x.SortOrder).HasColumnName("sort_order").HasDefaultValue(0);
            entity.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.HasIndex(x => x.Slug).IsUnique();
            entity.HasData(
                new CourseCategory { Id = 1, Name = "IELTS", Slug = "ielts", SortOrder = 1, IsActive = true },
                new CourseCategory { Id = 2, Name = "TOEIC", Slug = "toeic", SortOrder = 2, IsActive = true },
                new CourseCategory { Id = 3, Name = "TOEFL", Slug = "toefl", SortOrder = 3, IsActive = true },
                new CourseCategory { Id = 4, Name = "English Communication", Slug = "english-communication", SortOrder = 4, IsActive = true },
                new CourseCategory { Id = 5, Name = "Business English", Slug = "business-english", SortOrder = 5, IsActive = true },
                new CourseCategory { Id = 6, Name = "Academic English", Slug = "academic-english", SortOrder = 6, IsActive = true },
                new CourseCategory { Id = 7, Name = "Kids English", Slug = "kids-english", SortOrder = 7, IsActive = true },
                new CourseCategory { Id = 8, Name = "Grammar", Slug = "grammar", SortOrder = 8, IsActive = true },
                new CourseCategory { Id = 9, Name = "Pronunciation", Slug = "pronunciation", SortOrder = 9, IsActive = true },
                new CourseCategory { Id = 10, Name = "Listening", Slug = "listening", SortOrder = 10, IsActive = true },
                new CourseCategory { Id = 11, Name = "Speaking", Slug = "speaking", SortOrder = 11, IsActive = true },
                new CourseCategory { Id = 12, Name = "Reading", Slug = "reading", SortOrder = 12, IsActive = true },
                new CourseCategory { Id = 13, Name = "Writing", Slug = "writing", SortOrder = 13, IsActive = true },
                new CourseCategory { Id = 14, Name = "SAT English", Slug = "sat-english", SortOrder = 14, IsActive = true },
                new CourseCategory { Id = 15, Name = "Cambridge English", Slug = "cambridge-english", SortOrder = 15, IsActive = true }
            );
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("rooms");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Location).HasColumnName("location").HasMaxLength(200);
            entity.Property(x => x.Capacity).HasColumnName("capacity");
            entity.Property(x => x.Facilities).HasColumnName("facilities").HasColumnType("text");
            entity.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<ClassEntity>(entity =>
        {
            entity.ToTable("classes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.CourseId).HasColumnName("course_id").IsRequired();
            entity.Property(x => x.ClassCode).HasColumnName("class_code").HasMaxLength(30).IsRequired();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(200);
            entity.Property(x => x.StartDate).HasColumnName("start_date").HasColumnType("date");
            entity.Property(x => x.EndDate).HasColumnName("end_date").HasColumnType("date");
            entity.Property(x => x.MaxStudents).HasColumnName("max_students").HasDefaultValue(20);
            entity.Property(x => x.CurrentCount).HasColumnName("current_count").HasDefaultValue(0);
            entity.Property(x => x.Type).HasColumnName("type").HasColumnType("tinyint").HasDefaultValue((byte)1);
            entity.Property(x => x.Status).HasColumnName("status").HasColumnType("tinyint").HasDefaultValue((byte)1);
            entity.Property(x => x.TeacherId).HasColumnName("teacher_id");
            entity.Property(x => x.CreatedBy).HasColumnName("created_by");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => x.ClassCode).IsUnique();
            entity.HasOne<Course>().WithMany().HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.TeacherId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ClassSchedule>(entity =>
        {
            entity.ToTable("class_schedules");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.ClassId).HasColumnName("class_id").IsRequired();
            entity.Property(x => x.RoomId).HasColumnName("room_id");
            entity.Property(x => x.Weekday).HasColumnName("weekday").HasColumnType("tinyint").IsRequired();
            entity.Property(x => x.StartTime).HasColumnName("start_time").HasColumnType("time").IsRequired();
            entity.Property(x => x.EndTime).HasColumnName("end_time").HasColumnType("time").IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => new { x.ClassId, x.Weekday });
            entity.HasOne<ClassEntity>().WithMany().HasForeignKey(x => x.ClassId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Room>().WithMany().HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ClassSession>(entity =>
        {
            entity.ToTable("class_sessions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.ClassId).HasColumnName("class_id").IsRequired();
            entity.Property(x => x.SessionDate).HasColumnName("session_date").HasColumnType("date").IsRequired();
            entity.Property(x => x.StartTime).HasColumnName("start_time").HasColumnType("time").IsRequired();
            entity.Property(x => x.EndTime).HasColumnName("end_time").HasColumnType("time").IsRequired();
            entity.Property(x => x.TeacherId).HasColumnName("teacher_id");
            entity.Property(x => x.RoomId).HasColumnName("room_id");
            entity.Property(x => x.Type).HasColumnName("type").HasColumnType("tinyint").HasDefaultValue((byte)1);
            entity.Property(x => x.OnlineLink).HasColumnName("online_link").HasColumnType("text");
            entity.Property(x => x.Status).HasColumnName("status").HasColumnType("tinyint").HasDefaultValue((byte)1);
            entity.Property(x => x.CreatedBy).HasColumnName("created_by");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => x.ClassId);
            entity.HasIndex(x => x.SessionDate);
            entity.HasIndex(x => x.TeacherId);
            entity.HasIndex(x => x.RoomId);
            entity.HasIndex(x => x.Status);
            entity.HasOne<ClassEntity>().WithMany().HasForeignKey(x => x.ClassId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.TeacherId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne<Room>().WithMany().HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ClassAttendance>(entity =>
        {
            entity.ToTable("class_attendances");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.ClassSessionId).HasColumnName("class_session_id").IsRequired();
            entity.Property(x => x.ClassStudentId).HasColumnName("class_student_id").IsRequired();
            entity.Property(x => x.IsAbsent).HasColumnName("is_absent");
            entity.Property(x => x.AbsentReason).HasColumnName("absent_reason").HasColumnType("text");
            entity.Property(x => x.RecordedBy).HasColumnName("recorded_by");
            entity.Property(x => x.RecordedAt).HasColumnName("recorded_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => x.ClassSessionId);
            entity.HasIndex(x => x.ClassStudentId);
            entity.HasOne<ClassSession>().WithMany().HasForeignKey(x => x.ClassSessionId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<ClassStudent>().WithMany().HasForeignKey(x => x.ClassStudentId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.RecordedBy).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("enrollments");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.StudentId).HasColumnName("student_id").IsRequired();
            entity.Property(x => x.ClassId).HasColumnName("class_id").IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasColumnType("tinyint").HasDefaultValue((byte)1);
            entity.Property(x => x.TuitionFee).HasColumnName("tuition_fee").HasPrecision(12, 2);
            entity.Property(x => x.Discount).HasColumnName("discount").HasPrecision(12, 2).HasDefaultValue(0m);
            entity.Property(x => x.DiscountReason).HasColumnName("discount_reason").HasColumnType("text");
            entity.Property(x => x.FinalFee).HasColumnName("final_fee").HasPrecision(12, 2);
            entity.Property(x => x.EnrolledBy).HasColumnName("enrolled_by");
            entity.Property(x => x.EnrolledAt).HasColumnName("enrolled_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(x => x.CompletedAt).HasColumnName("completed_at").HasColumnType("timestamp");
            entity.Property(x => x.Notes).HasColumnName("notes").HasColumnType("text");
            entity.HasIndex(x => new { x.StudentId, x.ClassId }).IsUnique();
            entity.HasOne<Student>().WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ClassEntity>().WithMany().HasForeignKey(x => x.ClassId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Staff>().WithMany().HasForeignKey(x => x.EnrolledBy).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ClassStudent>(entity =>
        {
            entity.ToTable("class_students");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.ClassId).HasColumnName("class_id").IsRequired();
            entity.Property(x => x.StudentId).HasColumnName("student_id").IsRequired();
            entity.Property(x => x.EnrollmentId).HasColumnName("enrollment_id").IsRequired();
            entity.Property(x => x.JoinedAt).HasColumnName("joined_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(x => x.LeftAt).HasColumnName("left_at").HasColumnType("timestamp");
            entity.Property(x => x.Status).HasColumnName("status").HasColumnType("tinyint").HasDefaultValue((byte)1);
            entity.Property(x => x.AddedBy).HasColumnName("added_by");
            entity.Property(x => x.RemovedBy).HasColumnName("removed_by");
            entity.Property(x => x.Note).HasColumnName("note").HasColumnType("text");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp");
            entity.HasIndex(x => new { x.ClassId, x.StudentId }).IsUnique();
            entity.HasIndex(x => x.ClassId);
            entity.HasIndex(x => x.StudentId);
            entity.HasIndex(x => x.EnrollmentId);
            entity.HasIndex(x => x.Status);
            entity.HasOne<ClassEntity>().WithMany().HasForeignKey(x => x.ClassId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Student>().WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Enrollment>().WithMany().HasForeignKey(x => x.EnrollmentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.AddedBy).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.RemovedBy).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("invoices");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.InvoiceNo).HasColumnName("invoice_no").HasMaxLength(50).IsRequired();
            entity.Property(x => x.EnrollmentId).HasColumnName("enrollment_id").IsRequired();
            entity.Property(x => x.StudentId).HasColumnName("student_id").IsRequired();
            entity.Property(x => x.SubtotalAmount).HasColumnName("subtotal_amount").HasPrecision(12, 2).IsRequired();
            entity.Property(x => x.DiscountAmount).HasColumnName("discount_amount").HasPrecision(12, 2).HasDefaultValue(0m);
            entity.Property(x => x.FinalAmount).HasColumnName("final_amount").HasPrecision(12, 2).IsRequired();
            entity.Property(x => x.PaidAmount).HasColumnName("paid_amount").HasPrecision(12, 2).HasDefaultValue(0m);
            entity.Property(x => x.RemainingAmount).HasColumnName("remaining_amount").HasPrecision(12, 2).HasDefaultValue(0m);
            entity.Property(x => x.DueDate).HasColumnName("due_date").HasColumnType("date");
            entity.Property(x => x.Status).HasColumnName("status").HasColumnType("tinyint").HasDefaultValue((byte)1);
            entity.Property(x => x.Note).HasColumnName("note").HasColumnType("text");
            entity.Property(x => x.CreatedBy).HasColumnName("created_by");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp");
            entity.HasIndex(x => x.InvoiceNo).IsUnique();
            entity.HasIndex(x => x.EnrollmentId);
            entity.HasIndex(x => x.StudentId);
            entity.HasIndex(x => x.Status);
            entity.HasOne<Enrollment>().WithMany().HasForeignKey(x => x.EnrollmentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Student>().WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.InvoiceId).HasColumnName("invoice_id").IsRequired();
            entity.Property(x => x.Amount).HasColumnName("amount").HasPrecision(12, 2).IsRequired();
            entity.Property(x => x.Method).HasColumnName("method").HasColumnType("tinyint");
            entity.Property(x => x.Status).HasColumnName("status").HasColumnType("tinyint").HasDefaultValue((byte)1);
            entity.Property(x => x.TransactionRef).HasColumnName("transaction_ref").HasMaxLength(200);
            entity.Property(x => x.Note).HasColumnName("note").HasColumnType("text");
            entity.Property(x => x.CollectedBy).HasColumnName("collected_by");
            entity.Property(x => x.PaidAt).HasColumnName("paid_at").HasColumnType("timestamp");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => x.InvoiceId);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.PaidAt);
            entity.HasOne<Invoice>().WithMany().HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Staff>().WithMany().HasForeignKey(x => x.CollectedBy).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<SalaryConfig>(entity =>
        {
            entity.ToTable("salary_configs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(x => x.SalaryType).HasColumnName("salary_type").HasColumnType("tinyint").IsRequired();
            entity.Property(x => x.BaseSalary).HasColumnName("base_salary").HasPrecision(12, 2).HasDefaultValue(0m);
            entity.Property(x => x.TeachingRate).HasColumnName("teaching_rate").HasPrecision(12, 2).HasDefaultValue(0m);
            entity.Property(x => x.ConvertedLeadRate).HasColumnName("converted_lead_rate").HasPrecision(12, 2).HasDefaultValue(0m);
            entity.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
            entity.Property(x => x.EffectiveTo).HasColumnName("effective_to").HasColumnType("date");
            entity.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.EffectiveFrom);
            entity.HasIndex(x => x.IsActive);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Payroll>(entity =>
        {
            entity.ToTable("payrolls");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(x => x.SalaryConfigId).HasColumnName("salary_config_id");
            entity.Property(x => x.Month).HasColumnName("month").HasColumnType("tinyint").IsRequired();
            entity.Property(x => x.Year).HasColumnName("year").IsRequired();
            entity.Property(x => x.BaseAmount).HasColumnName("base_amount").HasPrecision(12, 2).HasDefaultValue(0m);
            entity.Property(x => x.TeachingAmount).HasColumnName("teaching_amount").HasPrecision(12, 2).HasDefaultValue(0m);
            entity.Property(x => x.KpiAmount).HasColumnName("kpi_amount").HasPrecision(12, 2).HasDefaultValue(0m);
            entity.Property(x => x.BonusAmount).HasColumnName("bonus_amount").HasPrecision(12, 2).HasDefaultValue(0m);
            entity.Property(x => x.DeductionAmount).HasColumnName("deduction_amount").HasPrecision(12, 2).HasDefaultValue(0m);
            entity.Property(x => x.GrossAmount).HasColumnName("gross_amount").HasPrecision(12, 2).HasDefaultValue(0m);
            entity.Property(x => x.NetAmount).HasColumnName("net_amount").HasPrecision(12, 2).HasDefaultValue(0m);
            entity.Property(x => x.Status).HasColumnName("status").HasColumnType("tinyint").HasDefaultValue((byte)1);
            entity.Property(x => x.GeneratedAt).HasColumnName("generated_at").HasColumnType("timestamp");
            entity.Property(x => x.ConfirmedAt).HasColumnName("confirmed_at").HasColumnType("timestamp");
            entity.Property(x => x.PaidAt).HasColumnName("paid_at").HasColumnType("timestamp");
            entity.Property(x => x.Note).HasColumnName("note").HasColumnType("text");
            entity.Property(x => x.CreatedBy).HasColumnName("created_by");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => new { x.UserId, x.Month, x.Year }).IsUnique();
            entity.HasIndex(x => x.Status);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<SalaryConfig>().WithMany().HasForeignKey(x => x.SalaryConfigId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PayrollItem>(entity =>
        {
            entity.ToTable("payroll_items");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.PayrollId).HasColumnName("payroll_id").IsRequired();
            entity.Property(x => x.Type).HasColumnName("type").HasColumnType("tinyint").IsRequired();
            entity.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(10, 2);
            entity.Property(x => x.UnitAmount).HasColumnName("unit_amount").HasPrecision(12, 2);
            entity.Property(x => x.Amount).HasColumnName("amount").HasPrecision(12, 2).IsRequired();
            entity.Property(x => x.ReferenceType).HasColumnName("reference_type").HasMaxLength(100);
            entity.Property(x => x.ReferenceId).HasColumnName("reference_id");
            entity.Property(x => x.Description).HasColumnName("description").HasColumnType("text");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => x.PayrollId);
            entity.HasIndex(x => x.Type);
            entity.HasIndex(x => new { x.ReferenceType, x.ReferenceId });
            entity.HasOne<Payroll>().WithMany().HasForeignKey(x => x.PayrollId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StaffKpiRecord>(entity =>
        {
            entity.ToTable("staff_kpi_records");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.StaffId).HasColumnName("staff_id").IsRequired();
            entity.Property(x => x.LeadId).HasColumnName("lead_id");
            entity.Property(x => x.Month).HasColumnName("month").HasColumnType("tinyint").IsRequired();
            entity.Property(x => x.Year).HasColumnName("year").IsRequired();
            entity.Property(x => x.Type).HasColumnName("type").HasColumnType("tinyint").HasDefaultValue((byte)1);
            entity.Property(x => x.Quantity).HasColumnName("quantity").HasDefaultValue(1);
            entity.Property(x => x.UnitAmount).HasColumnName("unit_amount").HasPrecision(12, 2).HasDefaultValue(0m);
            entity.Property(x => x.TotalAmount).HasColumnName("total_amount").HasPrecision(12, 2).HasDefaultValue(0m);
            entity.Property(x => x.Note).HasColumnName("note").HasColumnType("text");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => x.StaffId);
            entity.HasIndex(x => x.LeadId);
            entity.HasIndex(x => new { x.StaffId, x.Month, x.Year });
            entity.HasOne<Staff>().WithMany().HasForeignKey(x => x.StaffId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Lead>().WithMany().HasForeignKey(x => x.LeadId).OnDelete(DeleteBehavior.SetNull);
        });

        base.OnModelCreating(modelBuilder);
    }
}
