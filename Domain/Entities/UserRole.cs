namespace app.Domain.Entities;

public class UserRole
{
    public ulong UserId { get; set; }
    public ulong RoleId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ulong? CreatedByUserId { get; set; }
    public ulong? UpdatedByUserId { get; set; }
    public DateTime? DeletedAt { get; set; }
}
