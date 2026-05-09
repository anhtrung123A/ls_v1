namespace app.Models.Auth;

public class RefreshTokenSession
{
    public long UserId { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
}
