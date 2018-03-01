using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtApp.Core.Interfaces
{
    public interface IEncryption
    {
        string GetHash(string inputString);
        string GetHash(string inputString, string encryptionKey);
        string Encrypt(string inputString);
        string Encrypt(string inputString, string encryptionKey);
        string Decrypt(string inputString);
        string Decrypt(string inputString, string encryptionKey);
    }
}
