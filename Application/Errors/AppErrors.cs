namespace app.Application.Errors;

public static class AppErrors
{
    public static class User
    {
        public const string FullnameRequired = "Fullname is required.";
        public const string EmailRequired = "Email is required.";
        public const string EmailAlreadyExists = "Email already exists.";
    }

    public static class External
    {
        public const string AuthServiceCallFailed = "Failed to call auth service.";
        public const string AuthGeneratedPasswordMissing = "Auth service did not return generated password.";
    }

    public static class System
    {
        public const string InternalServerError = "Internal server error.";
    }
}
