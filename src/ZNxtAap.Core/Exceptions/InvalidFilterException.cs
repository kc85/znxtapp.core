using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtAap.Core.Exceptions
{
    public class InvalidFilterException : ExceptionBase
    {
        public InvalidFilterException(int errorCode, string message, Exception ex = null)
            : base(errorCode, message, ex)
        {
        }
    }
}
