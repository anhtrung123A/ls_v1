using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using app.Common.Pagination;
using app.Data.EF;
using app.Data.EF.Entities;
using app.DTOs.Students;
using app.Services.Auth;
using app.Services.Email;

namespace app.Repositories.Students;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public StudentRepository(AppDbContext dbContext, IPasswordHasher passwordHasher, IEmailService emailService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    public async Task<PagedResponse<StudentResponse>> GetAllAsync(StudentListQuery query, CancellationToken cancellationToken = default)
    {
        var studentQuery = _dbContext.Students.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim().ToLowerInvariant();
            studentQuery = studentQuery.Where(x =>
                (x.StudentCode != null && x.StudentCode.ToLower().Contains(keyword)) ||
                (x.ParentName != null && x.ParentName.ToLower().Contains(keyword)) ||
                (x.ParentPhone != null && x.ParentPhone.ToLower().Contains(keyword)) ||
                (x.ParentEmail != null && x.ParentEmail.ToLower().Contains(keyword)));
        }

        if (query.Status.HasValue)
        {
            studentQuery = studentQuery.Where(x => x.Status == query.Status.Value);
        }

        if (query.Source.HasValue)
        {
            studentQuery = studentQuery.Where(x => x.Source == query.Source.Value);
        }

        if (query.AssignedStaffId.HasValue)
        {
            studentQuery = studentQuery.Where(x => x.AssignedStaffId == query.AssignedStaffId.Value);
        }

        studentQuery = ApplyOrdering(studentQuery, query.OrderBy, query.OrderDir);

        var totalItems = await studentQuery.LongCountAsync(cancellationToken);
        var skip = (query.Page - 1) * query.PageSize;

        var items = await studentQuery
            .Skip(skip)
            .Take(query.PageSize)
            .Select(MapToResponse())
            .ToListAsync(cancellationToken);

        return new PagedResponse<StudentResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize)
        };
    }

    public async Task<PagedResponse<MyClassResponse>> GetMyClassesAsync(long userId, MyClassListQuery query, CancellationToken cancellationToken = default)
    {
        var studentId = await GetStudentIdByUserIdAsync(userId, cancellationToken);

        var classStudentsQuery =
            from cs in _dbContext.ClassStudents.AsNoTracking()
            join c in _dbContext.Classes.AsNoTracking() on cs.ClassId equals c.Id
            join co in _dbContext.Courses.AsNoTracking() on c.CourseId equals co.Id
            where cs.StudentId == studentId
            select new { cs, c, co };

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim().ToLowerInvariant();
            classStudentsQuery = classStudentsQuery.Where(x =>
                (x.c.ClassCode != null && x.c.ClassCode.ToLower().Contains(keyword)) ||
                (x.c.Name != null && x.c.Name.ToLower().Contains(keyword)) ||
                (x.co.Name != null && x.co.Name.ToLower().Contains(keyword)));
        }

        if (query.Status.HasValue)
        {
            classStudentsQuery = classStudentsQuery.Where(x => x.c.Status == query.Status.Value);
        }

        if (query.CourseId.HasValue)
        {
            classStudentsQuery = classStudentsQuery.Where(x => x.c.CourseId == query.CourseId.Value);
        }

        if (query.Type.HasValue)
        {
            classStudentsQuery = classStudentsQuery.Where(x => x.c.Type == query.Type.Value);
        }

        var classOrderAsc = !string.Equals(query.OrderDir, "desc", StringComparison.OrdinalIgnoreCase);
        var classOrderBy = query.OrderBy?.Trim().ToLowerInvariant();
        classStudentsQuery = classOrderBy switch
        {
            "classcode" or "class_code" => classOrderAsc ? classStudentsQuery.OrderBy(x => x.c.ClassCode) : classStudentsQuery.OrderByDescending(x => x.c.ClassCode),
            "name" or "classname" or "class_name" => classOrderAsc ? classStudentsQuery.OrderBy(x => x.c.Name) : classStudentsQuery.OrderByDescending(x => x.c.Name),
            "startdate" or "start_date" => classOrderAsc ? classStudentsQuery.OrderBy(x => x.c.StartDate) : classStudentsQuery.OrderByDescending(x => x.c.StartDate),
            "status" => classOrderAsc ? classStudentsQuery.OrderBy(x => x.c.Status) : classStudentsQuery.OrderByDescending(x => x.c.Status),
            _ => classOrderAsc ? classStudentsQuery.OrderBy(x => x.c.CreatedAt) : classStudentsQuery.OrderByDescending(x => x.c.CreatedAt)
        };

        var totalItems = await classStudentsQuery.LongCountAsync(cancellationToken);
        var skip = (query.Page - 1) * query.PageSize;

        var items = await classStudentsQuery
            .Skip(skip)
            .Take(query.PageSize)
            .Select(x => new MyClassResponse
            {
                ClassStudentId = x.cs.Id,
                ClassId = x.c.Id,
                ClassCode = x.c.ClassCode,
                ClassName = x.c.Name,
                CourseId = x.co.Id,
                CourseName = x.co.Name,
                StartDate = x.c.StartDate,
                EndDate = x.c.EndDate,
                ClassStatus = x.c.Status,
                ClassStudentStatus = x.cs.Status,
                JoinedAt = x.cs.JoinedAt,
                LeftAt = x.cs.LeftAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResponse<MyClassResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize)
        };
    }

    public async Task<PagedResponse<MyClassScheduleResponse>> GetMyClassSchedulesAsync(long userId, MyClassScheduleListQuery query, CancellationToken cancellationToken = default)
    {
        var studentId = await GetStudentIdByUserIdAsync(userId, cancellationToken);

        var classIdsQuery = _dbContext.ClassStudents.AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .Select(x => x.ClassId)
            .Distinct();

        var schedulesQuery =
            from sc in _dbContext.ClassSchedules.AsNoTracking()
            join c in _dbContext.Classes.AsNoTracking() on sc.ClassId equals c.Id
            join r in _dbContext.Rooms.AsNoTracking() on sc.RoomId equals r.Id into roomJoin
            from room in roomJoin.DefaultIfEmpty()
            where classIdsQuery.Contains(sc.ClassId)
            select new { sc, c, room };

        if (query.ClassId.HasValue)
        {
            schedulesQuery = schedulesQuery.Where(x => x.sc.ClassId == query.ClassId.Value);
        }

        if (query.Weekday.HasValue)
        {
            schedulesQuery = schedulesQuery.Where(x => x.sc.Weekday == query.Weekday.Value);
        }

        var scheduleOrderAsc = string.Equals(query.OrderDir, "asc", StringComparison.OrdinalIgnoreCase);
        var scheduleOrderBy = query.OrderBy?.Trim().ToLowerInvariant();
        schedulesQuery = scheduleOrderBy switch
        {
            "classname" or "class_name" => scheduleOrderAsc ? schedulesQuery.OrderBy(x => x.c.Name) : schedulesQuery.OrderByDescending(x => x.c.Name),
            "starttime" or "start_time" => scheduleOrderAsc ? schedulesQuery.OrderBy(x => x.sc.StartTime) : schedulesQuery.OrderByDescending(x => x.sc.StartTime),
            "weekday" => scheduleOrderAsc ? schedulesQuery.OrderBy(x => x.sc.Weekday) : schedulesQuery.OrderByDescending(x => x.sc.Weekday),
            _ => scheduleOrderAsc ? schedulesQuery.OrderBy(x => x.sc.CreatedAt) : schedulesQuery.OrderByDescending(x => x.sc.CreatedAt)
        };

        var totalItems = await schedulesQuery.LongCountAsync(cancellationToken);
        var skip = (query.Page - 1) * query.PageSize;

        var items = await schedulesQuery
            .Skip(skip)
            .Take(query.PageSize)
            .Select(x => new MyClassScheduleResponse
            {
                ClassScheduleId = x.sc.Id,
                ClassId = x.sc.ClassId,
                ClassCode = x.c.ClassCode,
                ClassName = x.c.Name,
                Weekday = x.sc.Weekday,
                StartTime = x.sc.StartTime,
                EndTime = x.sc.EndTime,
                RoomId = x.room != null ? x.room.Id : null,
                RoomName = x.room != null ? x.room.Name : null,
                RoomLocation = x.room != null ? x.room.Location : null,
                CreatedAt = x.sc.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResponse<MyClassScheduleResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize)
        };
    }

    public async Task<PagedResponse<MyClassSessionResponse>> GetMyClassSessionsAsync(long userId, MyClassSessionListQuery query, CancellationToken cancellationToken = default)
    {
        var studentId = await GetStudentIdByUserIdAsync(userId, cancellationToken);

        var classIdsQuery = _dbContext.ClassStudents.AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .Select(x => x.ClassId)
            .Distinct();

        var sessionsQuery =
            from s in _dbContext.ClassSessions.AsNoTracking()
            join c in _dbContext.Classes.AsNoTracking() on s.ClassId equals c.Id
            join r in _dbContext.Rooms.AsNoTracking() on s.RoomId equals r.Id into roomJoin
            from room in roomJoin.DefaultIfEmpty()
            where classIdsQuery.Contains(s.ClassId)
            select new { s, c, room };

        if (query.ClassId.HasValue)
        {
            sessionsQuery = sessionsQuery.Where(x => x.s.ClassId == query.ClassId.Value);
        }

        if (query.SessionDateFrom.HasValue)
        {
            sessionsQuery = sessionsQuery.Where(x => x.s.SessionDate >= query.SessionDateFrom.Value);
        }

        if (query.SessionDateTo.HasValue)
        {
            sessionsQuery = sessionsQuery.Where(x => x.s.SessionDate <= query.SessionDateTo.Value);
        }

        if (query.Status.HasValue)
        {
            sessionsQuery = sessionsQuery.Where(x => x.s.Status == query.Status.Value);
        }

        var orderAsc = !string.Equals(query.OrderDir, "desc", StringComparison.OrdinalIgnoreCase);
        var orderBy = query.OrderBy?.Trim().ToLowerInvariant();
        sessionsQuery = orderBy switch
        {
            "classname" or "class_name" => orderAsc
                ? sessionsQuery.OrderBy(x => x.c.Name).ThenBy(x => x.s.SessionDate).ThenBy(x => x.s.StartTime).ThenBy(x => x.s.Id)
                : sessionsQuery.OrderByDescending(x => x.c.Name).ThenByDescending(x => x.s.SessionDate).ThenByDescending(x => x.s.StartTime).ThenByDescending(x => x.s.Id),
            "starttime" or "start_time" => orderAsc
                ? sessionsQuery.OrderBy(x => x.s.StartTime).ThenBy(x => x.s.SessionDate).ThenBy(x => x.s.Id)
                : sessionsQuery.OrderByDescending(x => x.s.StartTime).ThenByDescending(x => x.s.SessionDate).ThenByDescending(x => x.s.Id),
            "status" => orderAsc
                ? sessionsQuery.OrderBy(x => x.s.Status).ThenBy(x => x.s.SessionDate).ThenBy(x => x.s.StartTime).ThenBy(x => x.s.Id)
                : sessionsQuery.OrderByDescending(x => x.s.Status).ThenByDescending(x => x.s.SessionDate).ThenByDescending(x => x.s.StartTime).ThenByDescending(x => x.s.Id),
            _ => orderAsc ? sessionsQuery.OrderBy(x => x.s.SessionDate).ThenBy(x => x.s.StartTime) : sessionsQuery.OrderByDescending(x => x.s.SessionDate).ThenByDescending(x => x.s.StartTime)
        };

        var totalItems = await sessionsQuery.LongCountAsync(cancellationToken);
        var skip = (query.Page - 1) * query.PageSize;

        var items = await sessionsQuery
            .Skip(skip)
            .Take(query.PageSize)
            .Select(x => new MyClassSessionResponse
            {
                ClassSessionId = x.s.Id,
                ClassId = x.s.ClassId,
                ClassCode = x.c.ClassCode,
                ClassName = x.c.Name,
                SessionDate = x.s.SessionDate,
                StartTime = x.s.StartTime,
                EndTime = x.s.EndTime,
                TeacherId = x.s.TeacherId,
                RoomId = x.s.RoomId,
                RoomName = x.room != null ? x.room.Name : null,
                RoomLocation = x.room != null ? x.room.Location : null,
                Status = x.s.Status
            })
            .ToListAsync(cancellationToken);

        return new PagedResponse<MyClassSessionResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize)
        };
    }

    public async Task<StudentResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Students
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(MapToResponse())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateAssignedStaffAsync(request.AssignedStaffId, cancellationToken);

        var student = new Student
        {
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Address = request.Address,
            ParentName = request.ParentName,
            ParentPhone = request.ParentPhone,
            ParentEmail = request.ParentEmail,
            Source = request.Source,
            AssignedStaffId = request.AssignedStaffId,
            Status = request.Status ?? 1,
            EnrolledAt = request.EnrolledAt,
            Notes = request.Notes
        };

        _dbContext.Students.Add(student);
        await _dbContext.SaveChangesAsync(cancellationToken);

        student.StudentCode = $"STD0000{student.Id}";
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(student.Id, cancellationToken)
            ?? throw new InvalidOperationException("Cannot read newly created student.");
    }

    public async Task<StudentResponse?> UpdateAsync(long id, UpdateStudentRequest request, CancellationToken cancellationToken = default)
    {
        var student = await _dbContext.Students.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (student is null)
        {
            return null;
        }

        await ValidateAssignedStaffAsync(request.AssignedStaffId, cancellationToken);

        student.DateOfBirth = request.DateOfBirth;
        student.Gender = request.Gender;
        student.Address = request.Address;
        student.ParentName = request.ParentName;
        student.ParentPhone = request.ParentPhone;
        student.ParentEmail = request.ParentEmail;
        student.Source = request.Source;
        student.AssignedStaffId = request.AssignedStaffId;
        student.Status = request.Status;
        student.EnrolledAt = request.EnrolledAt;
        student.Notes = request.Notes;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<StudentResponse?> CreateUserAsync(long id, CancellationToken cancellationToken = default)
    {
        var student = await _dbContext.Students.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (student is null)
        {
            return null;
        }

        if (student.UserId.HasValue)
        {
            throw new InvalidOperationException("Student already linked to a user.");
        }

        var lead = await _dbContext.Leads
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ConvertedTo == student.Id, cancellationToken);

        if (lead is null)
        {
            throw new InvalidOperationException("Cannot create user because converted lead is not found.");
        }

        if (string.IsNullOrWhiteSpace(lead.Email))
        {
            throw new InvalidOperationException("Lead email is required to create user.");
        }

        var email = lead.Email.Trim().ToLowerInvariant();
        var emailExisted = await _dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken);
        if (emailExisted)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var tempPassword = GenerateRandomPassword();

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var user = new User
        {
            FullName = lead.FullName.Trim(),
            Email = email,
            Phone = lead.Phone?.Trim(),
            Role = 4,
            PasswordHash = _passwordHasher.Hash(tempPassword),
            IsActive = true
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        student.UserId = user.Id;
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await _emailService.SendStudentAccountCreatedAsync(
            toEmail: user.Email,
            fullName: user.FullName ?? "Student",
            password: tempPassword,
            studentCode: student.StudentCode ?? $"STD0000{student.Id}",
            cancellationToken: cancellationToken);

        return await GetByIdAsync(student.Id, cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var student = await _dbContext.Students.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (student is null)
        {
            throw new KeyNotFoundException("Student not found.");
        }

        _dbContext.Students.Remove(student);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateAssignedStaffAsync(long? assignedStaffId, CancellationToken cancellationToken)
    {
        if (!assignedStaffId.HasValue)
        {
            return;
        }

        var staff = await _dbContext.Staff.AsNoTracking().FirstOrDefaultAsync(x => x.Id == assignedStaffId.Value, cancellationToken);
        if (staff is null)
        {
            throw new InvalidOperationException("Assigned staff does not exist.");
        }

        if (staff.Department != 1)
        {
            throw new InvalidOperationException("Student can only be assigned to sales staff (department 1).");
        }
    }

    private static string GenerateRandomPassword(int length = 12)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%";
        var bytes = RandomNumberGenerator.GetBytes(length);
        var result = new char[length];

        for (var i = 0; i < length; i++)
        {
            result[i] = chars[bytes[i] % chars.Length];
        }

        return new string(result);
    }

    private static System.Linq.Expressions.Expression<Func<Student, StudentResponse>> MapToResponse() => x => new StudentResponse
    {
        Id = x.Id,
        UserId = x.UserId,
        StudentCode = x.StudentCode,
        DateOfBirth = x.DateOfBirth,
        Gender = x.Gender,
        Address = x.Address,
        ParentName = x.ParentName,
        ParentPhone = x.ParentPhone,
        ParentEmail = x.ParentEmail,
        Source = x.Source,
        AssignedStaffId = x.AssignedStaffId,
        Status = x.Status,
        EnrolledAt = x.EnrolledAt,
        Notes = x.Notes,
        CreatedAt = x.CreatedAt
    };

    private static IQueryable<Student> ApplyOrdering(IQueryable<Student> query, string? orderBy, string? orderDir)
    {
        var isAsc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        var key = orderBy?.Trim().ToLowerInvariant();

        return key switch
        {
            "studentcode" => isAsc ? query.OrderBy(x => x.StudentCode) : query.OrderByDescending(x => x.StudentCode),
            "status" => isAsc ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
            "enrolledat" => isAsc ? query.OrderBy(x => x.EnrolledAt) : query.OrderByDescending(x => x.EnrolledAt),
            _ => isAsc ? query.OrderBy(x => x.CreatedAt) : query.OrderByDescending(x => x.CreatedAt)
        };
    }

    private async Task<long> GetStudentIdByUserIdAsync(long userId, CancellationToken cancellationToken)
    {
        var studentId = await _dbContext.Students.AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => (long?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!studentId.HasValue)
        {
            throw new UnauthorizedAccessException("Student profile not found for current user.");
        }

        return studentId.Value;
    }

}
