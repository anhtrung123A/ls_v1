namespace app.Data.EF.Entities;

public class Payment
{
    public long Id { get; set; }
    public long InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public byte? Method { get; set; } // cash 1 | bank_transfer 2 | card 3 | momo 4 | vnpay 5
    public byte Status { get; set; } = 1; // pending 1 | confirm 2
    public string? TransactionRef { get; set; }
    public string? Note { get; set; }
    public long? CollectedBy { get; set; } // staff
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
