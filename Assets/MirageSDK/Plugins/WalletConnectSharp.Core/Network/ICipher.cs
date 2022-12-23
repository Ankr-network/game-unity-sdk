using System.Text;
using System.Threading.Tasks;
using MirageSDK.WalletConnectSharp.Core.Models;

namespace MirageSDK.WalletConnectSharp.Core.Network
{
    public interface ICipher
    {   
        Task<EncryptedPayload> EncryptWithKey(byte[] key, string data, Encoding encoding = null);

        Task<string> DecryptWithKey(byte[] key, EncryptedPayload encryptedData, Encoding encoding = null);
    }
}