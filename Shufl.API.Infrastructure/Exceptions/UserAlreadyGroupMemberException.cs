namespace Shufl.API.Infrastructure.Exceptions
{
    public class UserAlreadyGroupMemberException : ExceptionBase
    {
        public UserAlreadyGroupMemberException(string errorMessage, string errorData) : base(errorMessage, errorData)
        {
            ErrorType = nameof(UserAlreadyGroupMemberException);
        }
    }
}
