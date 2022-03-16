﻿using System.Threading.Tasks;
 
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;

namespace Nethereum.RPC.Web3
{
    /// <summary>
    ///     web3_sha3
    ///     Returns Keccak-256 (not the standardized SHA3-256) of the given data.
    ///     Parameters
    ///     1. String - the data to convert into a SHA3 hash
    ///     params: [
    ///     '0x68656c6c6f20776f726c64'
    ///     ]
    ///     Returns
    ///     DATA - The SHA3 result of the given string.
    ///     Example
    ///     Request
    ///     curl -X POST --data '{"jsonrpc":"2.0","method":"web3_sha3","params":["0x68656c6c6f20776f726c64"],"id":64}'
    ///     Result
    ///     {
    ///     "id":64,
    ///     "jsonrpc": "2.0",
    ///     "result": "0x47173285a8d7341e5e972fc677286384f802f8ef42a5ec5f03bbfa254cb01fad"
    ///     }
    /// </summary>
    public class Web3Sha3 : RpcRequestResponseHandler<string>
    {
        public Web3Sha3(IClient client) : base(client, ApiMethods.web3_sha3.ToString())
        {
        }

        public Task<string> SendRequestAsync(HexUTF8String valueToConvertHexUtf8, object id = null)
        {
            return base.SendRequestAsync(id, valueToConvertHexUtf8);
        }

        public RpcRequest BuildRequest(HexUTF8String valueToConvert, object id = null)
        {
            return base.BuildRequest(id, valueToConvert);
        }
    }
}