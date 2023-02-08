using System;
using Newtonsoft.Json;

namespace AnkrSDK.MirageAPI.SmartContractManager.Data.GetContract
{
	public class ContractDeployment
	{
		[JsonProperty(PropertyName = "chainId")]
		public int ChainId { get; set; }

		[JsonProperty(PropertyName = "contractAddress")]
		public string ContractAddress { get; set; }

		[JsonProperty(PropertyName = "created")]
		public DateTime Created { get; set; }
	}
}