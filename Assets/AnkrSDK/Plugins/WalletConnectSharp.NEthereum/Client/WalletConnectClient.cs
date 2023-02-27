using System.Threading.Tasks;
using AnkrSDK.Plugins.WalletConnect.VersionShared;
using AnkrSDK.Plugins.WalletConnectSharp.Core.Models;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;

namespace AnkrSDK.Plugins.WalletConnectSharp.NEthereum.Client
{
    public class WalletConnectClient : ClientBase
    {
        private long _id;
        public IWalletConnectGenericRequester GenericRequester { get; }

        public WalletConnectClient(IWalletConnectGenericRequester genericRequester)
        {
            GenericRequester = genericRequester;
        }

        protected override async Task<RpcResponseMessage> SendAsync(RpcRequestMessage message, string route = null)
        {
            _id += 1;

            var request = new GenericJsonRpcRequest(_id, message);
            var response = await GenericRequester.SendGeneric(request);
            return response.ToRpcResponseMessage();
        }
    }
}