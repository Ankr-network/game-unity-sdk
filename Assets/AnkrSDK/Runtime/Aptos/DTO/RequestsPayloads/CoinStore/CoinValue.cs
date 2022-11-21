using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class CoinValue
	{
		[JsonProperty(PropertyName = "value")]
		public string Value;
	}
}