using Newtonsoft.Json;

namespace AnkrSDK.WebGL.DTO
{
	public class WebGLCallAnswer<TReturnType>
	{
		[JsonProperty(PropertyName = "result")]
		public TReturnType Result { get; set; }
	}
}