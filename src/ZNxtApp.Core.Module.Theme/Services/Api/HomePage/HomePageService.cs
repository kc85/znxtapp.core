﻿using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.Theme.Services.Api.HomePage
{
    public class HomePageService : ViewBaseService
    {
        public HomePageService(ParamContainer requestParam) : base(requestParam)
        {
        }

        public string Index()
        {
            return ContentHandler.GetStringContent(Route.TemplateURL);
        }
    }
}