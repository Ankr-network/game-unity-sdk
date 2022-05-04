using Newtonsoft.Json;

namespace AnkrSDK.WebGL
{
	public class WebGLMessageDTO
	{
		[JsonProperty]
		public string id;
		[JsonProperty]
		public WebGLMessageStatus status;
		[JsonProperty]
		public string payload;
	}
}