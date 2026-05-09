namespace app.DTOs.Payrolls;

public class PayrollDetailResponse
{
    public required PayrollResponse Payroll { get; init; }
    public required IReadOnlyList<PayrollItemResponse> PayrollItems { get; init; }
}
