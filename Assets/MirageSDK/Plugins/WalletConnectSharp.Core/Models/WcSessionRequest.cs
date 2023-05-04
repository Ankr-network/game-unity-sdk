using MirageSDK.WalletConnect.VersionShared.Models;
using Newtonsoft.Json;

namespace MirageSDK.WalletConnectSharp.Core.Models
{
    public class WcSessionRequest : JsonRpcRequest
    {
        [JsonProperty("params")]
        public WcSessionRequestRequestParams[] parameters;

        public WcSessionRequest(ClientMeta clientMeta, string clientId, int chainId = 1) : base()
        {
            id = clientId.GetHashCode();
            Method = "wc_sessionRequest";
            this.parameters = new[]
            {
                new WcSessionRequestRequestParams()
                {
                    peerId = clientId,
                    chainId = chainId,
                    peerMeta = clientMeta
                }
            };
        }

        public class WcSessionRequestRequestParams
        {
            public string peerId;
            public ClientMeta peerMeta;
            public int chainId;
        }
    }
}