using System;
using System.Linq;
using System.Collections.Generic;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.Core.Consts
{
    public static partial class CommonConst
    {
        public static class Filters
        {

            public const string IS_OVERRIDE_FILTER = "{" + CommonConst.CommonField.IS_OVERRIDE + " : " + CommonConst.CommonValue.FALSE + "}";
        }
    }
}