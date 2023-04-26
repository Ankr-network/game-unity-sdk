using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MirageSDK.WalletConnect.VersionShared.Data;
using MirageSDK.WalletConnect.VersionShared.Models.DeepLink;
using MirageSDK.WalletConnect.VersionShared.Models.DeepLink.Helpers;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace MirageSDK.WalletConnect.VersionShared.Utils
{
	public static class WalletDownloadHelper
	{
		private static Dictionary<string, WalletEntry> _walletEntriesCache;

		private static LocalWalletEntriesCollectionSO _localWalletEntries;

		public static async UniTask<WalletEntry> FindWalletEntryByName(string walletName, bool invalidateCache = false)
		{
			var supportedWallets = await FetchWalletList(downloadImages: false, invalidateCache: invalidateCache);
			var walletEntry =
				supportedWallets.Values.FirstOrDefault(predicate: a =>
					string.Equals(a: a.name, b: walletName,
						comparisonType: StringComparison.InvariantCultureIgnoreCase));

			return walletEntry;
		}

		public static async UniTask<WalletEntry> FindWalletEntry(Wallets wallet, bool invalidateCache = false)
		{
			var walletName = wallet.GetWalletName();
			return await FindWalletEntryByName(walletName: walletName, invalidateCache: invalidateCache);
		}

		public static async UniTask<Dictionary<string, WalletEntry>> FetchWalletList(bool downloadImages,
			bool invalidateCache = false)
		{
			if (invalidateCache)
			{
				_walletEntriesCache = null;
			}

			if (_walletEntriesCache != null)
			{
				//if wallet already cached but it does not have images loaded then load them before returning
				if (downloadImages)
				{
					foreach (var entry in _walletEntriesCache.Values)
					{
						if (!entry.AllImagesLoaded)
						{
							await entry.DownloadImages();
						}
					}
				}

				return _walletEntriesCache;
			}

			using (var webRequest = UnityWebRequest.Get(uri: "https://registry.walletconnect.org/data/wallets.json"))
			{
				// Request and wait for the desired page.
				await webRequest.SendWebRequest();

				#if UNITY_2020_2_OR_NEWER
				if (webRequest.result != UnityWebRequest.Result.Success)
				{
					Debug.Log(message: "Error Getting Wallet Info: " + webRequest.error);
					return null;
				}
				#else
				if (webRequest.isHttpError || webRequest.isNetworkError)
				{
					Debug.Log("Error Getting Wallet Info: " + webRequest.error);
					return null;
				}
				#endif

				var json = webRequest.downloadHandler.text;

				var supportedWallets = JsonConvert.DeserializeObject<Dictionary<string, WalletEntry>>(value: json);

				var filteredSupportedWallets = GetAllSupportedWallets(walletConnectSupportedWallets: supportedWallets);
				filteredSupportedWallets = ExtendSupportedWallets(filteredSupportedWallets);
				
				if (!downloadImages)
				{
					return filteredSupportedWallets;
				}
				
				if (filteredSupportedWallets != null)
				{
					var listOfTasks = new List<UniTask>();
					foreach (var wallet in filteredSupportedWallets.Values)
					{
						var downloadTask = wallet.DownloadImages();
						listOfTasks.Add(downloadTask);
					}

					await UniTask.WhenAll(listOfTasks);
				}

				_walletEntriesCache = filteredSupportedWallets;

				return filteredSupportedWallets;
			}
		}

		private static Dictionary<string, WalletEntry> GetAllSupportedWallets(
			Dictionary<string, WalletEntry> walletConnectSupportedWallets)
		{
			if (walletConnectSupportedWallets == null)
			{
				return null;
			}
			
			var walletsSupportedBySDK = WalletDataHelper.GetSupportedWalletNames();
			return walletConnectSupportedWallets
				.Where(predicate: w => walletsSupportedBySDK.Contains(value: w.Value.name))
				.ToDictionary(keySelector: i => i.Key, elementSelector: i => i.Value);
		}

		private static Dictionary<string, WalletEntry> ExtendSupportedWallets(Dictionary<string, WalletEntry> wallets)
		{
			if (_localWalletEntries == null)
			{
				_localWalletEntries = Resources.Load<LocalWalletEntriesCollectionSO>("LocalWalletEntriesCollection");
			}
			
			var walletNames = _localWalletEntries.LocallyConfiguredWalletNames;
			foreach(var additionalWalletName in walletNames)
			{
				wallets[additionalWalletName] = _localWalletEntries.GetWalletEntryByName(additionalWalletName);
			}

			return wallets;
		}
	}
}