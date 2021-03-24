using Shufl.API.Infrastructure.Enums;

namespace Shufl.API.Infrastructure.Exceptions
{
    public class InvalidTokenException : ExceptionBase
    {
        public InvalidTokenType InvalidTokenType;
        public InvalidTokenException(InvalidTokenType invalidTokenType, string errorData) : base("The provided token is invalid", errorData) 
        {
            InvalidTokenType = invalidTokenType;
            ErrorType = nameof(InvalidTokenException);
        }
    }
}
