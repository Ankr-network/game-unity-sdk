using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class MoveModuleBytecode
	{
		[JsonProperty(PropertyName = "bytecode")]
		public string Bytecode;
		[JsonProperty(PropertyName = "abi")]
		public MoveModule ABI;
	}
}