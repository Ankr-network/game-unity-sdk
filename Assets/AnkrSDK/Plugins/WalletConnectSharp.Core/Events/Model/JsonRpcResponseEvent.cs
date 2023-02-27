using AnkrSDK.Plugins.WalletConnect.VersionShared.Models;
using AnkrSDK.Plugins.WalletConnectSharp.Core.Models;

namespace AnkrSDK.Plugins.WalletConnectSharp.Core.Events.Model
{
    public class JsonRpcResponseEvent<T> : GenericEvent<T> where T : JsonRpcResponse
    {
    }
}