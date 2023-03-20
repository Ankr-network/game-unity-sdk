using Newtonsoft.Json;

namespace MirageSDK.WebGL.DTO
{
	public class WebGLCallAnswer<TReturnType>
	{
		[JsonProperty(PropertyName = "result")]
		public TReturnType Result { get; set; }
	}
}