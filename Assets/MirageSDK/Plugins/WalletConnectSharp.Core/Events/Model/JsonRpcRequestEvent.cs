using MirageSDK.Plugins.WalletConnectSharp.Core.Models;

namespace MirageSDK.Plugins.WalletConnectSharp.Core.Events.Model
{
    public class JsonRpcRequestEvent<T> : GenericEvent<T> where T : JsonRpcRequest
    {
    }
}