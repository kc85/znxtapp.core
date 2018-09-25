namespace ZNxtApp.Core.Interfaces
{
    public interface IModuleUninstaller
    {
        bool Uninstall(string moduleName, IHttpContextProxy httpProxy);
    }
}