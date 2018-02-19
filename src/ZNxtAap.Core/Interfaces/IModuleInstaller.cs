namespace ZNxtAap.Core.Interfaces
{
    public interface IModuleInstaller
    {
        bool Install(string moduleName,IHttpContextProxy httpProxy);
    }
}