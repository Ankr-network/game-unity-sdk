using System;
using Newtonsoft.Json;

namespace AnkrSDK.MirageAPI.SmartContractManager.Data.AllContracts
{
	[Serializable]
	public class SCMContractIDPair
	{
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }
		[JsonProperty(PropertyName = "id")]
		public string ID { get; set; }
	}
}