using UnityEngine;

namespace MirageSDK.Data
{
	[CreateAssetMenu(fileName = "Provider Information", menuName = "MirageSDK/Provider Information")]
	public class ProviderInformationSO : ScriptableObject
	{
		[SerializeField] private string _httpProviderURL;
		[SerializeField] private string _wsProviderURL;

		public string HttpProviderURL => _httpProviderURL;
		public string WsProviderURL => _wsProviderURL;

		public bool IsValid =>
			!string.IsNullOrWhiteSpace(_httpProviderURL)
			&& !string.IsNullOrWhiteSpace(_wsProviderURL);
	}
}