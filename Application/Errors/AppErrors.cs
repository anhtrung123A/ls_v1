namespace app.Application.Errors;

public static class AppErrors
{
    public static class Auth
    {
        public const string TokenSubjectMissing = "Token is invalid: subject claim is missing.";
        public const string TokenEmailMissing = "Token is invalid: email claim is missing.";
        public const string TokenExpired = "Token has expired.";
        public const string TokenInvalid = "Token is invalid.";
        public const string Unauthorized = "Unauthorized.";
    }

    public static class User
    {
        public const string LastnameRequired = "Lastname is required.";
        public const string EmailRequired = "Email is required.";
        public const string EmailAlreadyExists = "Email already exists.";
        public const string UserNotFoundByEmail = "User not found by token email.";
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
