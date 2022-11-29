using AnkrSDK.Aptos.Converters;
using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class DirectWriteSet : WriteSet
	{
		[JsonProperty(PropertyName = "changes")]
		[JsonConverter(typeof(WriteSetChangeConverter))]
		public WriteSetChange[] Changes;
		[JsonProperty(PropertyName = "events")]
		public Event[] Events;
	}
}