using AnkrSDK.WalletConnect.VersionShared.Models;

namespace AnkrSDK.WalletConnectSharp.Core.Events.Model
{
    public class JsonRpcRequestEvent<T> : GenericEvent<T> where T : JsonRpcRequest
    {
    }
}