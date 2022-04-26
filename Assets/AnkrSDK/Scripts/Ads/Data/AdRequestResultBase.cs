using System;
using Newtonsoft.Json;

namespace AnkrSDK.Ads.Data
{
	[Serializable]
	public abstract class AdRequestResultBase
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "code")]
		public ResponseCodeType Code { get; set; }
	}
}