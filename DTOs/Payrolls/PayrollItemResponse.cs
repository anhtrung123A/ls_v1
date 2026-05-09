namespace app.DTOs.Payrolls;

public class PayrollItemResponse
{
    public long Id { get; init; }
    public long PayrollId { get; init; }
    public byte Type { get; init; }
    public decimal? Quantity { get; init; }
    public decimal? UnitAmount { get; init; }
    public decimal Amount { get; init; }
    public string? ReferenceType { get; init; }
    public long? ReferenceId { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
}
