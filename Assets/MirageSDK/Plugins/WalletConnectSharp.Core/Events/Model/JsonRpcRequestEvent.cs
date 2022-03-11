using MirageSDK.WalletConnectSharp.Core.Models;

namespace MirageSDK.WalletConnectSharp.Core.Events.Model
{
    public class JsonRpcRequestEvent<T> : GenericEvent<T> where T : JsonRpcRequest
    {
    }
}