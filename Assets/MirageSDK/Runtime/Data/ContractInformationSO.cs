using UnityEngine;

namespace MirageSDK.Data
{
	[CreateAssetMenu(fileName = "Contract Information", menuName = "MirageSDK/Contract Information")]
	public class ContractInformationSO : ScriptableObject
	{
		[SerializeField] private string _abi;
		[SerializeField] private string _contractAddress;

		public string ABI => _abi;
		public string ContractAddress => _contractAddress;

		public bool IsValid =>
			!string.IsNullOrWhiteSpace(_contractAddress)
			&& !string.IsNullOrWhiteSpace(_abi);
	}
}