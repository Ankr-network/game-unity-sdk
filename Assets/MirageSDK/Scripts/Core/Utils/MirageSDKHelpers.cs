using System.Numerics;
using System.Text;
using MirageSDK.Core.Infrastructure;
using UnityEngine.Networking;

namespace MirageSDK.Core.Utils
{
	public static class MirageSDKHelpers
	{
		public static string StringToBigInteger(string value)
		{
			var bnValue = BigInteger.Parse(value);
			return "0x" + bnValue.ToString("X");
		}

		public static UnityWebRequest SendJSON(string url, string json)
		{
			var requestU = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
			var bytes = GetBytes(json);
			var uH = new UploadHandlerRaw(bytes);
			requestU.uploadHandler = uH;
			requestU.SetRequestHeader("Content-Type", "application/json");
			requestU.downloadHandler = new DownloadHandlerBuffer();
			return requestU;
		}

		private static byte[] GetBytes(string str)
		{
			var bytes = Encoding.UTF8.GetBytes(str);
			return bytes;
		}

		public static string GetURLFromNetworkNameEnum(NetworkNameEnum networkNameEnum)
		{
			string url = "";
			
			switch (networkNameEnum)
			{
				case NetworkNameEnum.Ethereum:
					break;
				case NetworkNameEnum.EthereumRinkebyTestNet:
					break;
				case  NetworkNameEnum.BinanceSmartChain:
					url = "https://metamask.app.link/dapp/change-network-mirage.surge.sh?network=bsc";
					break;
				case  NetworkNameEnum.BinanceSmartChainTestNet:
					url = "https://metamask.app.link/dapp/change-network-mirage.surge.sh?network=bsc_test";
					break;
			}

			return url;
		} 
	}
}