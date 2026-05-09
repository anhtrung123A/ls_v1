namespace app.DTOs.Payrolls;

public class UpdatePayrollStatusRequest
{
    // draft 1 | confirmed 2 | paid 3 | cancelled 4
    public byte Status { get; set; }
    public string? Note { get; set; }
}
