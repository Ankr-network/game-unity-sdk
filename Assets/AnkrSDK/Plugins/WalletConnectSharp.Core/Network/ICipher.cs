using System.Text;
using AnkrSDK.Plugins.WalletConnectSharp.Core.Models;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Plugins.WalletConnectSharp.Core.Network
{
    public interface ICipher
    {   
        UniTask<EncryptedPayload> EncryptWithKey(byte[] key, string data, Encoding encoding = null);

        UniTask<string> DecryptWithKey(byte[] key, EncryptedPayload encryptedData, Encoding encoding = null);
    }
}