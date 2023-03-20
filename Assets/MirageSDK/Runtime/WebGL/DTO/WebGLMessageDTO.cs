using Newtonsoft.Json;

namespace MirageSDK.WebGL.DTO
{
	internal class WebGLMessageDTO
	{
		[JsonProperty] internal string id;

		[JsonProperty] internal WebGLMessageStatus status;

		[JsonProperty] internal string payload;
	}
}