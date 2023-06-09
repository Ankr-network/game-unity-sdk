using Newtonsoft.Json;

namespace MirageSDK.UseCases.MirageNFT
{
	public class NftPropertyDTO
	{
		[JsonProperty("id")]
		public string Id { get; private set; }
		[JsonProperty("nftTokenId")]
		public string NftTokenId { get; private set; }
		[JsonProperty("type")]
		public string Type { get; private set; }
		[JsonProperty("name")]
		public string Name { get; private set; }
		[JsonProperty("value")]
		public string Value { get; private set; }
	}
}