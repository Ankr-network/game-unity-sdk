using System.Collections.Generic;

namespace AnkrSDK.Aptos.DTO
{
	public class OpenAPIConfig
	{
		public string Base { get; set; }
		public string Version { get; set; }
		public bool? WithCredentials { get; set; }
		public string Credentials { get; set; }
		public string Token { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public Dictionary<string, string> Headers { get; set; }
	}
}