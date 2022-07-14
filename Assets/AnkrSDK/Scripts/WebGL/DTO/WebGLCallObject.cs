using Newtonsoft.Json;

namespace AnkrSDK.WebGL.DTO
{
	public class WebGLCallObject
	{
		[JsonProperty(PropertyName = "path")]
		public string Path { get; set; }
		[JsonProperty(PropertyName = "args")]
		public object[] Args { get; set; }
	}
}