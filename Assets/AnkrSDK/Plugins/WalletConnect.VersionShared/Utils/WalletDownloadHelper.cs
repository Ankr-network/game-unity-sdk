using System.Collections.Generic;
using System.Linq;
using AnkrSDK.WalletConnect.VersionShared.Models.DeepLink;
using AnkrSDK.WalletConnect.VersionShared.Models.DeepLink.Helpers;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AnkrSDK.WalletConnect.VersionShared.Utils
{
	public static class WalletDownloadHelper
	{
		public static async UniTask<Dictionary<string, WalletEntry>> FetchWalletList(bool downloadImages)
		{
			using (var webRequest = UnityWebRequest.Get("https://registry.walletconnect.org/data/wallets.json"))
			{
				// Request and wait for the desired page.
				await webRequest.SendWebRequest();

			#if UNITY_2020_2_OR_NEWER
				if (webRequest.result != UnityWebRequest.Result.Success)
				{
					Debug.Log("Error Getting Wallet Info: " + webRequest.error);
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

				var supportedWallets = JsonConvert.DeserializeObject<Dictionary<string, WalletEntry>>(json);

				if (!downloadImages)
				{
					return supportedWallets;
				}

				var filteredSupportedWallets = GetAllSupportedWallets(supportedWallets);
				foreach (var wallet in filteredSupportedWallets.Values)
				{
					await wallet.DownloadImages();
				}

				return filteredSupportedWallets;
			}
		}

		private static Dictionary<string, WalletEntry> GetAllSupportedWallets(
			Dictionary<string, WalletEntry> walletconnectSupportedWallets)
		{
			var walletsSupportedBySDK = WalletNameHelper.GetSupportedWalletNames();
			return walletconnectSupportedWallets
				.Where(w => walletsSupportedBySDK.Contains(w.Value.name))
				.ToDictionary(i => i.Key, i => i.Value);
		}
	}
}