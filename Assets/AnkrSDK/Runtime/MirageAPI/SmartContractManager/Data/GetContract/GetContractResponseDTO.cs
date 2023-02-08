using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AnkrSDK.MirageAPI.SmartContractManager.Data.GetContract
{
	[Serializable]
	public class GetContractResponseDTO
	{
		[JsonProperty(PropertyName = "name")] public string ContractName { get; set; }

		[JsonProperty(PropertyName = "abi")] public string ContractABI { get; set; }

		[JsonProperty(PropertyName = "deployments")]
		public List<ContractDeployment> Deployments { get; set; }

		public ContractDeployment Contract => Deployments?.FirstOrDefault();
	}
}