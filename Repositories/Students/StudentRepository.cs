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
}
