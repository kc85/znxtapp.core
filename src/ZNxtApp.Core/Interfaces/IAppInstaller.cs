using ZNxtApp.Core.Enums;

namespace ZNxtApp.Core.Interfaces
{
    public interface IAppInstaller
    {
        void Install(IHttpContextProxy httpProxy);

        AppInstallStatus Status { get; }
    }
}