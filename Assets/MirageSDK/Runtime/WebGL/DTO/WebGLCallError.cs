using Newtonsoft.Json;

namespace MirageSDK.WebGL.DTO
{
	public class WebGLCallError
	{
		[JsonProperty("message")]
		private string _message;

		[JsonIgnore]
		public string Message => _message;
	}
}