using System.Collections.Generic;
using AnkrSDK.WalletConnectSharp.Unity.Models.DeepLink;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AnkrSDK.WalletConnectSharp.Unity.Utils
{
	public static class WalletDownloadHelper
	{
		public static async UniTask<Dictionary<string, AppEntry>> FetchWalletList(bool downloadImages)
		{
			using (var webRequest =
				UnityWebRequest.Get("https://registry.walletconnect.org/data/wallets.json"))
			{
				// Request and wait for the desired page.
				await webRequest.SendWebRequest();

				if (webRequest.isHttpError || webRequest.isNetworkError)
				{
					Debug.Log("Error Getting Wallet Info: " + webRequest.error);
					return null;
				}

				var json = webRequest.downloadHandler.text;

				var supportedWallets = JsonConvert.DeserializeObject<Dictionary<string, AppEntry>>(json);

				if (downloadImages)
				{
					foreach (var wallet in supportedWallets.Values)
					{
						await wallet.DownloadImages();
					}
				}

				return supportedWallets;
			}
		}
	}
}