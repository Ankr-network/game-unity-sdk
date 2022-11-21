using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class MoveScriptBytecode
	{
		[JsonProperty(PropertyName = "bytecode")]
		public string Bytecode;
		[JsonProperty(PropertyName = "abi")]
		public MoveFunction ABI;
	}
}