namespace ZNxtApp.Core.Interfaces
{
    public interface ISessionProvider
    {
        T GetValue<T>(string key);

        void ResetSession();

        void SetValue<T>(string key, T value);
    }
}