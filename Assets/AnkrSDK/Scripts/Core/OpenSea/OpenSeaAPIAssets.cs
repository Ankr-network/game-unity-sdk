using System;
using System.Collections.Generic;
using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.OpenSea.Data;
using AnkrSDK.Core.OpenSea.Helpers;
using AnkrSDK.Core.Utils;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AnkrSDK.Core.OpenSea
{
	public static class OpenSeaAPIAssets
	{
		private const string EndPoint = "https://rinkeby-api.opensea.io/api";
		private const string TestNetEndPoint = "https://api.opensea.io/api";
		private const string Version = "/v1";
		private const string GETAssets = "/assets";
		private const string GETAsset = "/asset";

		private static string _apiKey;
		private static bool _isTestNet = true;

		private static string BaseURL => $"{(_isTestNet ? EndPoint : TestNetEndPoint)}{Version}";
		private static string GetAssetsURL => $"{BaseURL}{GETAssets}";
		
		private static readonly TimeSpan DelayTimeSpan = TimeSpan.FromSeconds(1f);

		private static string GetSingleAssetURL(string contractAddress, string tokenId) =>
			$"{BaseURL}{GETAsset}/{contractAddress}/{tokenId}";

		public static void SetAPIKey(string apiKeyToSet)
		{
			_apiKey = apiKeyToSet;
		}

		public static void SetTestMode(bool isTestNet)
		{
			_isTestNet = isTestNet;
		}

		public static async UniTask<OpenSeaAsset> GetSingleAsset(string contractId, string tokenId, string ownerId,
			bool includeOrders)
		{
			var url = GetSingleAssetURL(contractId, tokenId);
			var keys = new Dictionary<string, string>();
			if (ownerId != string.Empty)
			{
				keys.Add("account_address", ownerId);
			}

			keys.Add("include_orders", includeOrders.ToString().ToLower());

			var singleAssetJSON = await CallAPI(url, keys);
			return JsonConvert.DeserializeObject<OpenSeaAsset>(singleAssetJSON);
		}

		public static UniTask<string> GetUserAssets(int offset = 0, int limit = 25,
			NFTOrderByType orderByType = NFTOrderByType.SalePrice)
		{
			return GetUserAssets(EthHandler.DefaultAccount, orderByType, offset, limit);
		}

		// Get assets from an owner
		public static UniTask<string> GetUserAssets(string ownerId, int offset = 0, int limit = 25,
			NFTOrderByType orderByType = NFTOrderByType.SalePrice)
		{
			return GetUserAssets(ownerId, orderByType, offset, limit);
		}

		private static UniTask<string> GetUserAssets(string ownerId, NFTOrderByType nftOrder, int offset, int limit)
		{
			var keys = new Dictionary<string, string>();
			if (ownerId != string.Empty)
			{
				keys.Add("owner", ownerId);
			}

			if (nftOrder != NFTOrderByType.None)
			{
				keys.Add("order_by", nftOrder.GetNameByType());
			}

			keys.Add("order_direction", "desc");
			keys.Add("offset", offset.ToString());
			keys.Add("limit", limit.ToString());

			return CallAPI(GetAssetsURL, keys);
		}

		private static async UniTask<string> CallAPI(string uri, Dictionary<string, string> keys)
		{
			const int maxTriesCount = 3;
			var tries = 0;

			var getURL = UriQueryBuilder.Build(uri, keys);

			Debug.Log($"Calling {getURL}");

			while (tries < maxTriesCount)
			{
				var webRequest = UnityWebRequest.Get(getURL);

				if (!string.IsNullOrEmpty(_apiKey))
				{
					const string apiKeyParam = "x-api-key";
					webRequest.SetRequestHeader(apiKeyParam, _apiKey);
				}
				else
				{
					Debug.LogWarning(
						"API Key is empty. This is OK for test environment," +
						" but you need to request a API Key for production;");
				}

				await webRequest.SendWebRequest();

				if (webRequest.result == UnityWebRequest.Result.Success)
				{
					return webRequest.downloadHandler.text;
				}

				tries++;
				await UniTask.Delay(DelayTimeSpan);
			}

			return null;
		}
	}
}