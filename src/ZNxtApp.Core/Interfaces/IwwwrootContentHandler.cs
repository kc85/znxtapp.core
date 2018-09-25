namespace ZNxtApp.Core.Interfaces
{
    public interface IwwwrootContentHandler
    {
        string GetStringContent(string path);

        byte[] GetContent(string path);
    }
}