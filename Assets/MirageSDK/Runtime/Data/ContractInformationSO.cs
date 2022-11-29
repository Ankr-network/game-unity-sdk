using UnityEngine;

namespace MirageSDK.Data
{
	[CreateAssetMenu(fileName = "Contract Information", menuName = "MirageSDK/Contract Information")]
	public class ContractInformationSO : ScriptableObject
	{
		[SerializeField] private string _abi;
		[SerializeField] private string _contractAddress;
		[SerializeField] private string _httpProviderURL;
		[SerializeField] private string _wsProviderURL;

		public string ABI => _abi;
		public string ContractAddress => _contractAddress;
		public string HttpProviderURL => _httpProviderURL;
		public string WsProviderURL => _wsProviderURL;
	}
}