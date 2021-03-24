using System;

namespace Shufl.API.Infrastructure.Exceptions
{
    public class ExceptionBase : Exception
    {
        public string ErrorMessage { get; private set; }
        public string ErrorData { get; private set; }
        public string ErrorType { get; protected set; }

        public ExceptionBase(string errorMessage, string errorData)
        {
            ErrorMessage = errorMessage;
            ErrorData = errorData;
        }
    }
}
