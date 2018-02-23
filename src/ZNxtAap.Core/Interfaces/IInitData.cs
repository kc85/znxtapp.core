using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtAap.Core.Interfaces
{
    public interface IInitData
    {
        DateTime InitDateTime { get; }
        string TransactionId { get; }
    }
}
