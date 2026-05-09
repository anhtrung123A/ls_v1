namespace app.Data.EF.Entities;

public class PayrollItem
{
    public long Id { get; set; }
    public long PayrollId { get; set; }
    public byte Type { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? UnitAmount { get; set; }
    public decimal Amount { get; set; }
    public string? ReferenceType { get; set; }
    public long? ReferenceId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
