using System;
using Newtonsoft.Json;

namespace AnkrSDK.WebGL.DTO
{
	public class WebGLCallObject
	{
		[JsonProperty(PropertyName = "path")]
		public string Path { get; set; }
		[JsonProperty(PropertyName = "args")]
		public Object[] Args { get; set; }
	}
}