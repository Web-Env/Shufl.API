namespace Shufl.API.Infrastructure.Exceptions
{
    public class SpotifyNoActiveDevicesException : ExceptionBase
    {
        public SpotifyNoActiveDevicesException(string errorMessage, string errorData) : base(errorMessage, errorData)
        {
            ErrorType = nameof(SpotifyNoActiveDevicesException);
        }
    }
}
