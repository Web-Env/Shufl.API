namespace Shufl.API.Infrastructure.Exceptions
{
    public class UserForbiddenException : ExceptionBase
    {
        public UserForbiddenException(string errorMessage, string errorData) : base(errorMessage, errorData)
        {
            ErrorType = nameof(UserForbiddenException);
        }
    }
}
