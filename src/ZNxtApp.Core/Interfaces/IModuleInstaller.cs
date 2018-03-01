namespace ZNxtApp.Core.Interfaces
{
    public interface IModuleInstaller
    {
        bool Install(string moduleName, IHttpContextProxy httpProxy, bool IsOverride = true);
    }
}