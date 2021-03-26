namespace Shufl.API.Infrastructure.Exceptions
{
    public class UserNotGroupMemberException : ExceptionBase
    {
        public UserNotGroupMemberException(string errorMessage, string errorData) : base(errorMessage, errorData)
        {
            ErrorType = nameof(UserNotGroupMemberException);
        }
    }
}
