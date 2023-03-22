using MirageSDK.WalletConnect.VersionShared.Models;

namespace MirageSDK.WalletConnectSharp.Core.Events.Model
{
    public class JsonRpcRequestEvent<T> : GenericEvent<T> where T : JsonRpcRequest
    {
    }
}