using System;
using System.Collections.Generic;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Web.Models;
namespace ZNxtApp.Core.Web.Interfaces
{
    public interface IEventSubscription
    {
        List<EventSubscriptionModel> GetSubscriptions(string eventName, ExecutionEventType eventType);
        void LoadSubscriptions(bool forceLoad = false);
    }
}
