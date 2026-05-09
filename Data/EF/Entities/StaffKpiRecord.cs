namespace app.Data.EF.Entities;

public class StaffKpiRecord
{
    public long Id { get; set; }
    public long StaffId { get; set; }
    public long? LeadId { get; set; }
    public byte Month { get; set; }
    public int Year { get; set; }
    public byte Type { get; set; } = 1;
    public int Quantity { get; set; } = 1;
    public decimal UnitAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
