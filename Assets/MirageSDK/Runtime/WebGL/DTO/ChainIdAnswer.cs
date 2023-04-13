using Nethereum.Hex.HexTypes;
using Newtonsoft.Json;

namespace MirageSDK.WebGL.DTO
{
	public class ChainIdAnswer
	{
		[JsonProperty(PropertyName = "chainId")]
		public HexBigInteger ChainId;
	}
}