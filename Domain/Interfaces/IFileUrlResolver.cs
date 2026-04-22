namespace app.Domain.Interfaces;

public interface IFileUrlResolver
{
    string? BuildPublicUrl(string? objectKey);
}
