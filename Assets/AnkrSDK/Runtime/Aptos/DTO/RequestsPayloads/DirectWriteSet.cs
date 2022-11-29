using AnkrSDK.Aptos.Converters;
using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class DirectWriteSet : WriteSet
	{
		[JsonProperty(PropertyName = "changes")]
		[JsonConverter(typeof(WriteSetChangeArrayConverter))]
		public WriteSetChange[] Changes;
		[JsonProperty(PropertyName = "events")]
		public Event[] Events;
	}
}