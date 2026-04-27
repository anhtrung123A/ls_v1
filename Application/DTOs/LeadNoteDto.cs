namespace app.Application.DTOs;

public class LeadNoteDto
{
    public ulong Id { get; set; }
    public ulong LeadId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Metadata { get; set; }
}
