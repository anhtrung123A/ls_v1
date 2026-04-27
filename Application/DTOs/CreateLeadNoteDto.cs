namespace app.Application.DTOs;

public class CreateLeadNoteDto
{
    public string Content { get; set; } = string.Empty;
    public string? Metadata { get; set; }
}
