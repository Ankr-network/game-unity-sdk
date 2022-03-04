using System.Text;
using System.Threading.Tasks;
using MirageSDK.Plugins.WalletConnectSharp.Core.Models;

namespace MirageSDK.Plugins.WalletConnectSharp.Core.Network
{
    public interface ICipher
    {   
        Task<EncryptedPayload> EncryptWithKey(byte[] key, string data, Encoding encoding = null);

        Task<string> DecryptWithKey(byte[] key, EncryptedPayload encryptedData, Encoding encoding = null);
    }
}