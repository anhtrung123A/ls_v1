namespace app.Domain.Entities;

public class BranchUser
{
    public ulong Id { get; set; }
    public ulong UserId { get; set; }
    public ulong BranchId { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ulong? CreatedByUserId { get; set; }
    public ulong? UpdatedByUserId { get; set; }
    public DateTime? DeletedAt { get; set; }
}
