﻿using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Infrastructure;

namespace Nethereum.RPC.Web3
{
    /// <Summary>
    ///     web3_clientVersion
    ///     Returns the current client version.
    ///     Parameters
    ///     none
    ///     Returns
    ///     String - The current client version
    ///     Curl Example
    ///     Request
    ///     curl -X POST --data '{"jsonrpc":"2.0","method":"web3_clientVersion","params":[],"id":67}'
    ///     Result
    ///     {
    ///     "id":67,
    ///     "jsonrpc":"2.0",
    ///     "result": "Mist/v0.9.3/darwin/go1.4.1"
    ///     }
    /// </Summary>
    public class Web3ClientVersion : GenericRpcRequestResponseHandlerNoParam<string>
    {
        public Web3ClientVersion(IClient client) : base(client, ApiMethods.web3_clientVersion.ToString())
        {
        }
    }
}