using System;
using Newtonsoft.Json;

namespace AnkrSDK.Ads.Data
{
	[Serializable]
	public class AdRequestResultResult : AdRequestResultBase
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "error")]
		public string Error { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "result")]
		public AdData AdData { get; set; }
	}
}