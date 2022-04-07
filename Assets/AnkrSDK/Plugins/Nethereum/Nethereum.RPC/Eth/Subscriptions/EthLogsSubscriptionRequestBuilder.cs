using System;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.RPC.Eth.Subscriptions
{
    public class EthLogsSubscriptionRequestBuilder:RpcRequestBuilder
    {
        public EthLogsSubscriptionRequestBuilder() : base(ApiMethods.eth_subscribe.ToString())
        {
        }

        public RpcRequest BuildRequest(NewFilterInput filterInput, string id)
        {
            if (id == null) id = Guid.NewGuid().ToString();
            return base.BuildRequest(id, "logs", filterInput);
        }
    }
}