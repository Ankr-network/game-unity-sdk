using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Core.Models;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;

namespace AnkrSDK.WalletConnectSharp.NEthereum.Client
{
    public class WalletConnectClient : ClientBase
    {
        private long _id;
        public IWalletConnectCommunicator Communicator { get; }

        public WalletConnectClient(IWalletConnectCommunicator communicator)
        {
            Communicator = communicator;
        }

        protected override async Task<RpcResponseMessage> SendAsync(RpcRequestMessage message, string route = null)
        {
            _id += 1;

            var request = new GenericJsonRpcRequest(_id, message);
            var response = await Communicator.Send<GenericJsonRpcRequest, GenericJsonRpcResponse>(request);
            return response.ToRpcResponseMessage();
        }
    }
}