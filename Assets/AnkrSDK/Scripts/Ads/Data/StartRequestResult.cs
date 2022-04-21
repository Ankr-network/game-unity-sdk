using System;
using Newtonsoft.Json;

namespace AnkrSDK.Ads.Data
{
	[Serializable]
	public class StartRequestResult
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "code")]
		public int Code { get; set; }
	}
}