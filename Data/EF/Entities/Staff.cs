namespace app.Data.EF.Entities;

public class Staff
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string? StaffCode { get; set; }
    // sales = 1, academic = 2, operation = 3, management = 4
    public byte? Department { get; set; }
    public string? Position { get; set; }
    public long? ManagerId { get; set; }
    public DateTime JoinedAt { get; set; }
}
