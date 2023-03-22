using System.Text;
using MirageSDK.WalletConnectSharp.Core.Models;
using Cysharp.Threading.Tasks;

namespace MirageSDK.WalletConnectSharp.Core.Network
{
    public interface ICipher
    {   
        UniTask<EncryptedPayload> EncryptWithKey(byte[] key, string data, Encoding encoding = null);

        UniTask<string> DecryptWithKey(byte[] key, EncryptedPayload encryptedData, Encoding encoding = null);
    }
}