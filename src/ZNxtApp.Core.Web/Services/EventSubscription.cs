using System.Collections.Generic;
using System.Linq;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Web.Interfaces;
using ZNxtApp.Core.Web.Models;

namespace ZNxtApp.Core.Web.Services
{
    public class EventSubscription : IEventSubscription
    {
        private static EventSubscription _eventSubscription;
        private static object _lockobj = new object();
        private List<EventSubscriptionModel> _events; 
        private IDBService _dbProxy;
        private readonly ILogger _logger;
        private EventSubscription(IDBService dbProxy, ILogger logger)
        {
            _logger = logger;
            _dbProxy = dbProxy;
            LoadSubscriptions();
        }
        public static EventSubscription GetInstance(IDBService _dbProxy, ILogger logger)
        {
            if (_eventSubscription == null)
            {
                lock (_lockobj)
                {
                    _eventSubscription = new EventSubscription(_dbProxy, logger);
                }
            }
            return _eventSubscription;
        }
        
        public List<EventSubscriptionModel> GetSubscriptions(string eventName, ExecutionEventType eventType)
        {
            return _events.Where(f => f.EventName == eventName && f.EventType == eventType).ToList();
        }
        public void LoadSubscriptions(bool forceLoad = false)
        {
            if (_events == null || forceLoad)
            {
                lock (_lockobj)
                {
                    _events = new List<EventSubscriptionModel>();
                    
                    var data = _dbProxy.Get(CommonConst.Collection.EVENT_SUBSCRIPTION,CommonConst.Filters.IS_OVERRIDE_FILTER);
                    foreach (var item in data)
                    {
                        EventSubscriptionModel eventModel = Newtonsoft.Json.JsonConvert.DeserializeObject<EventSubscriptionModel>(item.ToString());
                        _events.Add(eventModel);
                    }
                }
            }

        }
    }
}
