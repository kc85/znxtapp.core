using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtAap.Core.Interfaces
{
    public interface IModuleUninstaller
    {
        bool Uninstall(string moduleName, IHttpContextProxy httpProxy);
    }
}
