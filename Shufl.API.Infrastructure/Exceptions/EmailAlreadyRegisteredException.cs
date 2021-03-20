namespace Shufl.API.Infrastructure.Exceptions
{
    public class EmailAlreadyRegisteredException : ExceptionBase
    {
        public EmailAlreadyRegisteredException(string errorMessage, string errorData) : base(errorMessage, errorData) { }
    }
}
