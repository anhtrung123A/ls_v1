using Microsoft.EntityFrameworkCore;
using app.Data.EF;
using app.Data.EF.Entities;
using app.DTOs.Leads;
using app.Common.Pagination;

namespace app.Repositories.Leads;

public class LeadRepository : ILeadRepository
{
    private readonly AppDbContext _dbContext;

    public LeadRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<LeadResponse>> GetAllAsync(LeadListQuery query, CancellationToken cancellationToken = default)
    {
        var leadQuery = _dbContext.Leads.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim().ToLowerInvariant();
            leadQuery = leadQuery.Where(x =>
                x.FullName.ToLower().Contains(keyword) ||
                (x.Phone != null && x.Phone.ToLower().Contains(keyword)) ||
                (x.Email != null && x.Email.ToLower().Contains(keyword)));
        }

        if (query.Source.HasValue)
        {
            leadQuery = leadQuery.Where(x => x.Source == query.Source.Value);
        }

        if (query.Status.HasValue)
        {
            leadQuery = leadQuery.Where(x => x.Status == query.Status.Value);
        }

        if (query.AssignedTo.HasValue)
        {
            leadQuery = leadQuery.Where(x => x.AssignedTo == query.AssignedTo.Value);
        }

        leadQuery = ApplyOrdering(leadQuery, query.OrderBy, query.OrderDir);

        var totalItems = await leadQuery.LongCountAsync(cancellationToken);
        var skip = (query.Page - 1) * query.PageSize;

        var items = await leadQuery
            .Skip(skip)
            .Take(query.PageSize)
            .Select(MapToResponse())
            .ToListAsync(cancellationToken);

        return new PagedResponse<LeadResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize)
        };
    }

    public async Task<LeadResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Leads
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(MapToResponse())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<LeadResponse> CreateAsync(CreateLeadRequest request, CancellationToken cancellationToken = default)
    {
        var lead = new Lead
        {
            FullName = request.FullName.Trim(),
            Phone = request.Phone?.Trim(),
            Email = request.Email?.Trim().ToLowerInvariant(),
            Source = request.Source,
            Campaign = request.Campaign?.Trim(),
            Interest = request.Interest,
            Status = 1,
            Note = request.Note,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Leads.Add(lead);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(lead.Id, cancellationToken)
            ?? throw new InvalidOperationException("Cannot read newly created lead.");
    }

    public async Task<LeadResponse?> UpdateAsync(long id, UpdateLeadRequest request, CancellationToken cancellationToken = default)
    {
        var lead = await _dbContext.Leads.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (lead is null)
        {
            return null;
        }

        await ValidateAssignmentAsync(request.AssignedTo, cancellationToken);
        await ValidateConvertedStudentAsync(request.ConvertedTo, cancellationToken);

        lead.FullName = request.FullName.Trim();
        lead.Phone = request.Phone?.Trim();
        lead.Email = request.Email?.Trim().ToLowerInvariant();
        lead.Source = request.Source;
        lead.Campaign = request.Campaign?.Trim();
        lead.Interest = request.Interest;
        lead.Status = request.Status;
        lead.LostReason = request.LostReason;
        lead.AssignedTo = request.AssignedTo;
        lead.ConvertedTo = request.ConvertedTo;
        lead.Note = request.Note;
        lead.ConvertedAt = request.ConvertedAt;
        lead.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(lead.Id, cancellationToken);
    }

    public async Task<LeadResponse?> AssignAsync(long id, long? assignedTo, CancellationToken cancellationToken = default)
    {
        var lead = await _dbContext.Leads.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (lead is null)
        {
            return null;
        }

        await ValidateAssignmentAsync(assignedTo, cancellationToken);
        lead.AssignedTo = assignedTo;
        lead.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(lead.Id, cancellationToken);
    }

    public async Task<ConvertLeadResponse?> ConvertToStudentAsync(long id, CancellationToken cancellationToken = default)
    {
        var lead = await _dbContext.Leads.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (lead is null)
        {
            return null;
        }

        if (lead.Status == 5 && lead.ConvertedTo.HasValue)
        {
            throw new InvalidOperationException("Lead is already converted.");
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var student = new Student
        {
            UserId = null,
            Source = lead.Source,
            AssignedStaffId = lead.AssignedTo,
            Notes = lead.Note
        };

        _dbContext.Students.Add(student);
        await _dbContext.SaveChangesAsync(cancellationToken);

        student.StudentCode = $"STD0000{student.Id}";

        var convertedAt = DateTime.UtcNow;
        lead.Status = 5;
        lead.ConvertedAt = convertedAt;
        lead.ConvertedTo = student.Id;
        lead.UpdatedAt = convertedAt;

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new ConvertLeadResponse
        {
            LeadId = lead.Id,
            StudentId = student.Id,
            LeadStatus = lead.Status,
            ConvertedAt = convertedAt
        };
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var lead = await _dbContext.Leads.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (lead is null)
        {
            throw new KeyNotFoundException("Lead not found.");
        }

        _dbContext.Leads.Remove(lead);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateAssignmentAsync(long? assignedTo, CancellationToken cancellationToken)
    {
        if (!assignedTo.HasValue)
        {
            return;
        }

        var assignedStaff = await _dbContext.Staff
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == assignedTo.Value, cancellationToken);

        if (assignedStaff is null)
        {
            throw new InvalidOperationException("Assigned staff does not exist.");
        }

        if (assignedStaff.Department != 1)
        {
            throw new InvalidOperationException("Lead can only be assigned to sales staff (department 1).");
        }

        var role = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == assignedStaff.UserId)
            .Select(u => u.Role)
            .FirstOrDefaultAsync(cancellationToken);

        if (role != 2)
        {
            throw new InvalidOperationException("Assigned target must be staff role (role 2).");
        }
    }

    private async Task ValidateConvertedStudentAsync(long? convertedTo, CancellationToken cancellationToken)
    {
        if (!convertedTo.HasValue)
        {
            return;
        }

        var existed = await _dbContext.Students
            .AsNoTracking()
            .AnyAsync(s => s.Id == convertedTo.Value, cancellationToken);

        if (!existed)
        {
            throw new InvalidOperationException("Converted student does not exist.");
        }
    }

    private static System.Linq.Expressions.Expression<Func<Lead, LeadResponse>> MapToResponse() => lead => new LeadResponse
    {
        Id = lead.Id,
        FullName = lead.FullName,
        Phone = lead.Phone,
        Email = lead.Email,
        Source = lead.Source,
        Campaign = lead.Campaign,
        Interest = lead.Interest,
        Status = lead.Status,
        LostReason = lead.LostReason,
        AssignedTo = lead.AssignedTo,
        ConvertedTo = lead.ConvertedTo,
        Note = lead.Note,
        CreatedAt = lead.CreatedAt,
        UpdatedAt = lead.UpdatedAt,
        ConvertedAt = lead.ConvertedAt
    };

    private static IQueryable<Lead> ApplyOrdering(IQueryable<Lead> query, string? orderBy, string? orderDir)
    {
        var isAsc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        var key = orderBy?.Trim().ToLowerInvariant();

        return key switch
        {
            "fullname" => isAsc ? query.OrderBy(x => x.FullName) : query.OrderByDescending(x => x.FullName),
            "status" => isAsc ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
            "source" => isAsc ? query.OrderBy(x => x.Source) : query.OrderByDescending(x => x.Source),
            "updatedat" => isAsc ? query.OrderBy(x => x.UpdatedAt) : query.OrderByDescending(x => x.UpdatedAt),
            _ => isAsc ? query.OrderBy(x => x.CreatedAt) : query.OrderByDescending(x => x.CreatedAt)
        };
    }
}
