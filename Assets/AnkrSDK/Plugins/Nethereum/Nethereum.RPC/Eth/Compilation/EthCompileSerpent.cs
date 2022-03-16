using System;
using System.Threading.Tasks;
 
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace Nethereum.RPC.Eth.Compilation
{
    /// <Summary>
    ///     eth_compileSerpent
    ///     Returns compiled serpent code.
    ///     Parameters
    ///     String - The source code.
    ///     params: [
    ///     "/* some serpent */",
    ///     ]
    ///     Returns
    ///     DATA - The compiled source code.
    ///     Example
    ///     Request
    ///     curl -X POST --data '{"jsonrpc":"2.0","method":"eth_compileSerpent","params":["/* some serpent */"],"id":1}'
    ///     Result
    ///     {
    ///     "id":1,
    ///     "jsonrpc": "2.0",
    ///     "result":
    ///     "0x603880600c6000396000f3006001600060e060020a600035048063c6888fa114601857005b6021600435602b565b8060005260206000f35b600081600702905091905056"
    ///     // the compiled source code
    ///     }
    /// </Summary>
    public class EthCompileSerpent : RpcRequestResponseHandler<JObject>, IEthCompileSerpent
    {
        public EthCompileSerpent(IClient client) : base(client, ApiMethods.eth_compileSerpent.ToString())
        {
        }

        public Task<JObject> SendRequestAsync(string serpentCode, object id = null)
        {
            if (serpentCode == null) throw new ArgumentNullException(nameof(serpentCode));
            return base.SendRequestAsync(id, serpentCode);
        }

        public RpcRequest BuildRequest(string serpentCode, object id = null)
        {
            if (serpentCode == null) throw new ArgumentNullException(nameof(serpentCode));
            return base.BuildRequest(id, serpentCode);
        }
    }
}