using System;
using System.Numerics;
using System.Text;
using AnkrSDK.Core.Data;
using UnityEngine.Networking;

namespace AnkrSDK.Core.Utils
{
	public static class AnkrSDKHelper
	{
		public static string StringToBigInteger(string value)
		{
			var bnValue = BigInteger.Parse(value);
			return "0x" + bnValue.ToString("X");
		}

		public static UnityWebRequest GetUnityWebRequestFromJSON(string url, string json)
		{
			var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
			var bytes = GetBytes(json);
			var uH = new UploadHandlerRaw(bytes);
			request.uploadHandler = uH;
			request.SetRequestHeader("Content-Type", "application/json");
			request.downloadHandler = new DownloadHandlerBuffer();
			return request;
		}

		private static byte[] GetBytes(string str)
		{
			var bytes = Encoding.UTF8.GetBytes(str);
			return bytes;
		}

		public static string GetURLFromNetworkNameEnum(NetworkName networkName)
		{
			var url = "";
			
			switch (networkName)
			{
				case NetworkName.Ethereum:
					break;
				case NetworkName.EthereumRinkebyTestNet:
					break;
				case  NetworkName.BinanceSmartChain:
					url = "https://metamask.app.link/dapp/change-network-mirage.surge.sh?network=bsc";
					break;
				case  NetworkName.BinanceSmartChainTestNet:
					url = "https://metamask.app.link/dapp/change-network-mirage.surge.sh?network=bsc_test";
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(networkName), networkName, null);
			}

			return url;
		} 
	}
}