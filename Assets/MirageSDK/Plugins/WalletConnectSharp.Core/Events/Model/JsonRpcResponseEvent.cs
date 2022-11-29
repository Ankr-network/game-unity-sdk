using MirageSDK.WalletConnectSharp.Core.Models;

namespace MirageSDK.WalletConnectSharp.Core.Events.Model
{
    public class JsonRpcResponseEvent<T> : GenericEvent<T> where T : JsonRpcResponse
    {
    }
}