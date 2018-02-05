using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Enums;

namespace ZNxtAap.Core.Interfaces
{
    public interface IAppInstaller
    {
        void Install(IHttpContextProxy httpProxy);
        AppInstallStatus Status { get; }
    }
}
