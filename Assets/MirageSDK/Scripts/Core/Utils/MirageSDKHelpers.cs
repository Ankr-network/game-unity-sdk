using System.Numerics;
using System.Text;
using UnityEngine.Networking;

namespace MirageSDK.Core.Utils
{
	public static class MirageSDKHelpers
	{
		public static string ConvertNumber(string value)
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
	}
}