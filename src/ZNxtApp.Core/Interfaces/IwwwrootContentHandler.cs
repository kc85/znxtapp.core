﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtApp.Core.Interfaces
{
   public interface IwwwrootContentHandler
    {
        string GetStringContent(string path);
        byte[]  GetContent(string path);

    }
}
