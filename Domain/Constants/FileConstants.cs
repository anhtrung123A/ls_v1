namespace app.Domain.Constants;

public static class FileConstants
{
    public const int FileNameMaxLength = 255;
    public const int ContentTypeMaxLength = 100;
    public const int ChecksumMaxLength = 128;
    public const int BucketNameMaxLength = 100;
    public const int ObjectKeyMaxLength = 500;
    public const string AvatarFolder = "avatars";
    public const string BranchImageFolder = "branches";
    public const long AvatarMaxSizeBytes = 5 * 1024 * 1024;
    public static readonly string[] AllowedAvatarContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];
}
