using UnityEngine;

namespace AnkrSDK.Core.OpenSea
{
	public static class AnkrOpenSea
	{
		private const string EndPoint = "dapp://mirage-nft-market.surge.sh/#/item";

		private const int ChainId = 1;

		private static string GetSingleAssetURL(string contractAddress, string tokenId) =>
			$"{EndPoint}/{ChainId}/{contractAddress}/{tokenId}";

		public static void OpenSingleAssetInfoLink(string contractId, string tokenId)
		{
			var url = GetSingleAssetURL(contractId, tokenId);

			CallAPI(url);
		}

		private static void CallAPI(string url)
		{
			Debug.Log($"Calling {url}");

			Application.OpenURL(url);
		}
	}
}