using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class ScriptWriteSet : WriteSet
	{
		[JsonProperty(PropertyName = "execute_as")]
		public string ExecuteAs;
		[JsonProperty(PropertyName = "script")]
		public ScriptPayload Script;
	}
}