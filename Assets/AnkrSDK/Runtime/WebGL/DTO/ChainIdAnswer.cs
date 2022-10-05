using Nethereum.Hex.HexTypes;
using Newtonsoft.Json;

namespace AnkrSDK.WebGL.DTO
{
	public class ChainIdAnswer
	{
		[JsonProperty(PropertyName = "chainId")]
		public HexBigInteger ChainId;
	}
}