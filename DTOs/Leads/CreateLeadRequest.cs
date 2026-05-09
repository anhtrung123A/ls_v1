namespace app.DTOs.Leads;

public class CreateLeadRequest
{
    public required string FullName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public byte? Source { get; set; }
    public string? Campaign { get; set; }
    public string? Interest { get; set; }
    public string? Note { get; set; }
}
