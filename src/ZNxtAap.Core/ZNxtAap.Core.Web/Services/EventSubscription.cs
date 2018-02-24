using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.Interfaces;
using ZNxtAap.Core.Model;
using ZNxtAap.Core.Web.Interfaces;
using ZNxtAap.Core.Web.Models;

namespace ZNxtAap.Core.Web.Services
{
    public class EventSubscription : IEventSubscription
    {
        private static EventSubscription _eventSubscription;
        private static object _lockobj = new object();
        private List<EventModel> _events; 
        private IDBService _dbProxy;
        private readonly ILogger _logger;
        private EventSubscription(IDBService dbProxy, ILogger logger)
        {
            _logger = logger;
            _dbProxy = dbProxy;
            LoadEvents();
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
        
        public List<EventModel> GetEvent(string eventName)
        {
            return _events.Where(f => f.EventName == eventName).ToList();
        }
        public void LoadEvents(bool forceLoad = false)
        {
            if (_events == null || forceLoad)
            {
                lock (_lockobj)
                {
                    _events = new List<EventModel>();
                    _dbProxy.Collection = CommonConst.Collection.EVENT_SUBSCRIPTION;
                    var data = _dbProxy.Get(CommonConst.Filters.IS_OVERRIDE_FILTER);
                    foreach (var item in data)
                    {
                        EventModel eventModel = Newtonsoft.Json.JsonConvert.DeserializeObject<EventModel>(item.ToString());
                        _events.Add(eventModel);
                    }
                }
            }

        }
    }
}
