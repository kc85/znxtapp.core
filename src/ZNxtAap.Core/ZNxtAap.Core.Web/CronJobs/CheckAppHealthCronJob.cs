﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Model;
using ZNxtAap.Core.Services;

namespace ZNxtAap.Core.Web.CronJobs
{
    public class CheckAppHealthCronJob : CronServiceBase
    {
        public CheckAppHealthCronJob(ParamContainer pamamContainer)
            : base(pamamContainer)
        {

        }
        public int Check()
        {
            return 1;
        }
    }
}
