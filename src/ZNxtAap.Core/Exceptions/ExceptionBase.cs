using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtAap.Core.Exceptions
{
    public class ExceptionBase : Exception
    {
        public IList<string> ErrorMessages = new List<string>();
        public int ErrorCode { get; private set; }

        public ExceptionBase(int errorCode, string message, Exception ex = null)
            : base(message, ex)
        {
            ErrorCode = errorCode;
        }
    }
}
