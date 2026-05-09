using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using app.Data.EF;
using app.Data.EF.Entities;
using app.DTOs.Staff;
using app.Services.Auth;
using app.Services.Email;

namespace app.Repositories.Staff;

public class StaffRepository : IStaffRepository
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public StaffRepository(AppDbContext dbContext, IPasswordHasher passwordHasher, IEmailService emailService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    public async Task<StaffResponse> CreateAsync(CreateStaffRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var emailExisted = await _dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
        if (emailExisted)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        if (request.Department != 4 && !request.ManagerId.HasValue)
        {
            throw new InvalidOperationException("Manager is required when department is not management (4).");
        }

        var managerId = request.Department == 4 ? null : request.ManagerId;

        if (managerId.HasValue)
        {
            var managerExisted = await _dbContext.Staff.AnyAsync(s => s.Id == managerId.Value, cancellationToken);
            if (!managerExisted)
            {
                throw new InvalidOperationException("Manager does not exist.");
            }
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        var generatedPassword = GenerateRandomPassword();

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            Phone = request.Phone?.Trim(),
            Role = 2,
            PasswordHash = _passwordHasher.Hash(generatedPassword),
            IsActive = true
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var staff = new Data.EF.Entities.Staff
        {
            UserId = user.Id,
            Department = request.Department,
            Position = request.Position?.Trim(),
            ManagerId = managerId
        };

        _dbContext.Staff.Add(staff);
        await _dbContext.SaveChangesAsync(cancellationToken);

        staff.StaffCode = $"EMP0000{staff.Id}";
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await _emailService.SendStaffAccountCreatedAsync(
            toEmail: user.Email,
            fullName: user.FullName ?? "Staff",
            password: generatedPassword,
            staffCode: staff.StaffCode,
            cancellationToken: cancellationToken);

        return new StaffResponse
        {
            Id = staff.Id,
            UserId = user.Id,
            StaffCode = staff.StaffCode,
            Department = staff.Department,
            Position = staff.Position,
            ManagerId = staff.ManagerId,
            JoinedAt = staff.JoinedAt,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role
        };
    }

    public async Task DeleteAsync(long staffId, CancellationToken cancellationToken = default)
    {
        var staff = await _dbContext.Staff.FirstOrDefaultAsync(s => s.Id == staffId, cancellationToken);
        if (staff is null)
        {
            throw new KeyNotFoundException("Staff not found.");
        }

        var hasChildren = await _dbContext.Staff.AnyAsync(s => s.ManagerId == staffId, cancellationToken);
        if (hasChildren)
        {
            throw new InvalidOperationException("Cannot delete staff because they are manager of other staff.");
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        _dbContext.Staff.Remove(staff);

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == staff.UserId, cancellationToken);
        if (user is not null)
        {
            _dbContext.Users.Remove(user);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
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
}
