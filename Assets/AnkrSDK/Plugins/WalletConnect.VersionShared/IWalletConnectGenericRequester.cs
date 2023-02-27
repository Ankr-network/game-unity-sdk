using AnkrSDK.Plugins.WalletConnectSharp.Core.Models;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Plugins.WalletConnect.VersionShared
{
    public interface IWalletConnectGenericRequester
    {
        bool ConnectionPending { get; }
        UniTask<GenericJsonRpcResponse> SendGeneric(GenericJsonRpcRequest genericRequest);
    }
}