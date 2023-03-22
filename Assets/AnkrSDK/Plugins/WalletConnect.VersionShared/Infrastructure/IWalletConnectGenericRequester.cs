using AnkrSDK.WalletConnect.VersionShared.Models;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.WalletConnect.VersionShared.Infrastructure
{
    public interface IWalletConnectGenericRequester
    {
        bool CanSendRequests { get; }
        UniTask<GenericJsonRpcResponse> GenericRequest(GenericJsonRpcRequest genericRequest);
    }
}