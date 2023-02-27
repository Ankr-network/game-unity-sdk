using AnkrSDK.Plugins.WalletConnect.VersionShared.Models;

namespace AnkrSDK.Plugins.WalletConnectSharp.Core.Events.Model
{
    public class JsonRpcRequestEvent<T> : GenericEvent<T> where T : JsonRpcRequest
    {
    }
}