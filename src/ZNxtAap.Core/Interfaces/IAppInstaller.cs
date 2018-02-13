using ZNxtAap.Core.Enums;

namespace ZNxtAap.Core.Interfaces
{
    public interface IAppInstaller
    {
        void Install(IHttpContextProxy httpProxy);

        AppInstallStatus Status { get; }
    }
}