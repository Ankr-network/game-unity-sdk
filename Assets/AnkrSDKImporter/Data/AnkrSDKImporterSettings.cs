using System.Collections.Generic;
using UnityEngine;

namespace AnkrSDKImporter.Data
{
	[CreateAssetMenu(fileName = "AnkrSDKImporterSettings", menuName = "AnkrSDK/ImporterSettings")]
	public class AnkrSDKImporterSettings : ScriptableObject
	{
		[SerializeField] private List<PackageDataEntry> _packageDataList = new List<PackageDataEntry>();
		[SerializeField] private List<string> _registryScopes = new List<string>();
		[SerializeField] private string _openUpmRegistryName;
		[SerializeField] private string _openUpmRegistryUrl;
		
		public string OpenUpmRegistryName => _openUpmRegistryName;
		public string OpenUpmRegistryUrl => _openUpmRegistryUrl;
		
		public IEnumerable<PackageDataEntry> GetPackageDataEntries()
		{
			return _packageDataList;
		}

		public IEnumerable<string> GetRegistryScopes()
		{
			return _registryScopes;
		}
		
		
	}
}
