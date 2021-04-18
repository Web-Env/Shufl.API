namespace Shufl.API.Infrastructure.Exceptions
{
    public class SpotifyNotLinkedException : ExceptionBase
    {
        public SpotifyNotLinkedException(string errorMessage, string errorData) : base(errorMessage, errorData)
        {
            ErrorType = nameof(SpotifyNotLinkedException);
        }
    }
}
