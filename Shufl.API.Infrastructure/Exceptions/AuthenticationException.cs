namespace Shufl.API.Infrastructure.Exceptions
{
    public class AuthenticationException : ExceptionBase
    {
        public AuthenticationException(string errorMessage, string errorData) : base(errorMessage, errorData) { }
    }
}
