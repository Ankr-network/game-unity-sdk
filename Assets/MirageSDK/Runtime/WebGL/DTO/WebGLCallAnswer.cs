using Newtonsoft.Json;

namespace MirageSDK.WebGL.DTO
{
	public class WebGLCallAnswer<TReturnType>
	{
		[JsonProperty("result")]
		public TReturnType Result { get; set; }

		[JsonProperty("error")]
		private WebGLCallError _error { get; set; }

		[JsonIgnore]
		public WebGLCallError Error => _error;

		[JsonIgnore]
		public bool IsError => _error != null;
	}
}