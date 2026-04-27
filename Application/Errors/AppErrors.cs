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

    public static class Branch
    {
        public const string NameRequired = "Branch name is required.";
        public const string NotFound = "Branch not found.";
    }

    public static class BranchUser
    {
        public const string NotFound = "Branch user not found.";
        public const string AlreadyExists = "Branch user already exists.";
        public const string InvalidStatus = "Branch user status is invalid.";
    }

    public static class Lead
    {
        public const string NotFound = "Lead not found.";
        public const string FirstNameRequired = "Lead first name is required.";
        public const string FullNameRequired = "Lead full name is required.";
        public const string PhoneNumberAlreadyExists = "Lead phonenumber already exists.";
    }

    public static class LeadNote
    {
        public const string NotFound = "Lead note not found.";
        public const string ContentRequired = "Lead note content is required.";
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
