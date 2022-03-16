using AnkrSDK.WalletConnectSharp.Core.Models;

namespace AnkrSDK.WalletConnectSharp.Core.Events.Model
{
    public class JsonRpcResponseEvent<T> : GenericEvent<T> where T : JsonRpcResponse
    {
    }
}