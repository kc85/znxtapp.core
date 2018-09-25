using System;

namespace ZNxtApp.Core.Interfaces
{
    public interface IInitData
    {
        DateTime InitDateTime { get; }
        string TransactionId { get; }
    }
}