using System;
using System.Collections.Generic;
using ZNxtAap.Core.Model;
using ZNxtAap.Core.Web.Models;
namespace ZNxtAap.Core.Web.Interfaces
{
    public interface IEventSubscription
    {
        List<EventModel> GetEvent(string eventName);
        void LoadEvents(bool forceLoad = false);
    }
}
