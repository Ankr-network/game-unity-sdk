using Newtonsoft.Json;

namespace MirageSDK.WebGL.DTO
{
	public class WebGLCallObject
	{
		[JsonProperty(PropertyName = "path")]
		public string Path { get; set; }
		[JsonProperty(PropertyName = "args")]
		public object[] Args { get; set; }
	}
}