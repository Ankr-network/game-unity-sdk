using System;
using Newtonsoft.Json;

namespace AnkrSDK.Ads.Data
{
	[Serializable]
	public class AdRequestResult
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "result")]
		public bool Result { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "uuid")]
		public string UUID { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "expire_at")]
		public long ExpireAtTimestamp { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "texture_url")]
		public string TextureURL { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "texture_width")]
		public int TextureWidth { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "texture_height")]
		public int TextureHeight { get; set; }
	}
}