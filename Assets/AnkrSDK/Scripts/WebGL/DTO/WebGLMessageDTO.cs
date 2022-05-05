using Newtonsoft.Json;

namespace AnkrSDK.WebGL.DTO
{
	internal class WebGLMessageDTO
	{
		[JsonProperty] internal string id;

		[JsonProperty] internal WebGLMessageStatus status;

		[JsonProperty] internal string payload;
	}
}