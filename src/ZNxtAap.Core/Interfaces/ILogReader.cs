﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtAap.Core.Interfaces
{
    public interface ILogReader
    {
        JArray GetLogs(string transactionId);
    }
}
