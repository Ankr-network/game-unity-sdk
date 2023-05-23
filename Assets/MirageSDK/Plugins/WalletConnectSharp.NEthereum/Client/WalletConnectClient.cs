using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MirageSDK.WalletConnect.VersionShared.Infrastructure;
using MirageSDK.WalletConnect.VersionShared.Models;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;

namespace MirageSDK.WalletConnectSharp.NEthereum.Client
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
            var response = await GenericRequester.GenericRequest(request);
            return response.ToRpcResponseMessage();
        }

        protected override async Task<RpcResponseMessage[]> SendAsync(RpcRequestMessage[] requests)
        {
            var tasks = new List<UniTask<GenericJsonRpcResponse>>();
            
            foreach (var message in requests)
            {
                _id += 1;
                var request = new GenericJsonRpcRequest(_id, message);
                var task = GenericRequester.GenericRequest(request);
                tasks.Add(task);
            }

            var responses = await UniTask.WhenAll(tasks);
            var rpcResponseMessages = responses.Select(r => r.ToRpcResponseMessage()).ToArray();
            return rpcResponseMessages;
        }
    }
}