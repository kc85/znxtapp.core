using System;
using System.Collections.Generic;

namespace ZNxtApp.Core.Exceptions
{
    public class ClientValidationError : ExceptionBase
    {
        public ClientValidationError(int errorCode, string message, IList<string> errorMessages = null, Exception ex = null)
            : base(errorCode, message, ex)
        {
            ErrorMessages = errorMessages;
        }
    }
}