using System;
using System.Collections.Generic;

namespace ZNxtApp.Core.Exceptions
{
    public class SchemaValidationException : ExceptionBase
    {
        public IList<string> Messages = new List<string>();

        public SchemaValidationException(int errorCode, IList<string> errorMessages, Exception ex = null)
            : base(errorCode, string.Empty, ex)
        {
            Messages = errorMessages;
        }
    }
}