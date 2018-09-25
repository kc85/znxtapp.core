namespace ZNxtApp.Core.Interfaces
{
    public interface IViewEngine
    {
        string Compile(string inputTemplete, string key, object dataModel);
    }
}