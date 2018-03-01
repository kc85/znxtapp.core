﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Model;

namespace ZNxtApp.Core.Interfaces
{
    public interface IActionExecuter
    {
        object Exec(RoutingModel route, ParamContainer helper);
        object Exec(string execultAssembly, string executeType, string executeMethod, ParamContainer helper);
    }
}