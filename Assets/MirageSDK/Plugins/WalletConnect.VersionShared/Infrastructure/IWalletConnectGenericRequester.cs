using MirageSDK.WalletConnect.VersionShared.Models;
using Cysharp.Threading.Tasks;

namespace MirageSDK.WalletConnect.VersionShared.Infrastructure
{
    public interface IWalletConnectGenericRequester
    {
        bool CanSendRequests { get; }
        UniTask<GenericJsonRpcResponse> GenericRequest(GenericJsonRpcRequest genericRequest);
    }
}