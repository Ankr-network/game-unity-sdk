using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class CoinStoreType
	{
		[JsonProperty(PropertyName = "coin")]
		public CoinValue Coin;
	}
}