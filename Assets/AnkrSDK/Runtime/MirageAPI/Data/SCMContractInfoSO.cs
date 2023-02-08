using UnityEngine;

namespace AnkrSDK.MirageAPI.Data
{
	public class SCMContractInfoSO : ScriptableObject
	{
		[SerializeField] private string _contractName;
		[SerializeField] private string _abi;
		[SerializeField] private string _contractAddress;

		public string ContractName => _contractName;
		public string ABI => _abi;
		public string ContractAddress => _contractAddress;

		public void Setup(string contractName, string abi, string contractAddress)
		{
			_contractName = contractName;
			_abi = abi;
			_contractAddress = contractAddress;
		}
	}
}