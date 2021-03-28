using Shufl.API.Infrastructure.Enums;

namespace Shufl.API.Infrastructure.Exceptions
{
    public class InvalidTokenException : ExceptionBase
    {
        public InvalidTokenType InvalidTokenType { get; }
        public InvalidTokenException(InvalidTokenType invalidTokenType, string errorMessage) : base(errorMessage, "The provided token is invalid")
        {
            InvalidTokenType = invalidTokenType;
            ErrorType = nameof(InvalidTokenException);
        }
    }
}
