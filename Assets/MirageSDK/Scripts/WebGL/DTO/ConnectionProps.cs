using MirageSDK.Data;
using Newtonsoft.Json;

namespace MirageSDK.WebGL.DTO
{
	public class ConnectionProps
	{
		[JsonProperty]
		public string wallet;
		[JsonProperty]
		public EthereumNetwork chain;
	}
}