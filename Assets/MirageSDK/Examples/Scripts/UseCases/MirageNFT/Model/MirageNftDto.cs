using System.Collections.Generic;
using Newtonsoft.Json;

namespace MirageSDK.UseCases.MirageNFT
{
	public class MirageNftDto
	{
		[JsonProperty("id")]
		public string Id { get; private set; }
		
		[JsonProperty("nftProperties")]
		public List<NftPropertyDTO> NftProperties { get; private set; }
		
		[JsonProperty("image")]
		public string Image { get; private set; }

		public NftPropertyDTO FindPropertyByName(string name)
		{
			foreach (var property in NftProperties)
			{
				if (property.Name == name)
				{
					return property;
				}
			}

			return null;
		}
	}
}