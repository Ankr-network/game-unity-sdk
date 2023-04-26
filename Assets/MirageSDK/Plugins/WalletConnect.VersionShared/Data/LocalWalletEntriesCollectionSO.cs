using System.Collections.Generic;
using MirageSDK.WalletConnect.VersionShared.Models.DeepLink;
using Newtonsoft.Json;
using UnityEngine;

namespace MirageSDK.WalletConnect.VersionShared.Data
{
	[CreateAssetMenu(fileName = "LocalWalletEntriesCollection", menuName = "MirageSDK/WalletConnect/LocalWalletEntriesCollection")]
	public class LocalWalletEntriesCollectionSO : ScriptableObject
	{
		[SerializeField]
		private List<LocalWalletEntry> _localWalletEntries = new List<LocalWalletEntry>();

		private Dictionary<string, WalletEntry> _walletEntriesCache = new Dictionary<string, WalletEntry>();

		public IEnumerable<string> LocallyConfiguredWalletNames
		{
			get
			{
				foreach (var localWalletEntry in _localWalletEntries)
				{
					yield return localWalletEntry.WalletName;
				}
			}
		}

		public WalletEntry GetWalletEntryByName(string walletName)
		{
			if (_walletEntriesCache.TryGetValue(walletName, out var cachedWalletEntry))
			{
				return cachedWalletEntry;
			}
			
			foreach (var localEntry in _localWalletEntries)
			{
				if (localEntry.WalletName.Equals(walletName))
				{
					if (!string.IsNullOrWhiteSpace(localEntry.WalletEntryJson))
					{
						var walletEntry = JsonConvert.DeserializeObject<WalletEntry>(localEntry.WalletEntryJson);
						return walletEntry;
					}
				}
			}

			return null;
		}
	}
}