namespace Shufl.API.Infrastructure.Exceptions
{
    public class UsernameAlreadyRegisteredException : ExceptionBase
    {
        public UsernameAlreadyRegisteredException(string errorMessage, string errorData) : base(errorMessage, errorData)
        {
            ErrorType = nameof(UsernameAlreadyRegisteredException);
        }
    }
}
