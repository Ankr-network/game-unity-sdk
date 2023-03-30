using System.Numerics;
using System.Text;
using UnityEngine.Networking;

namespace MirageSDK.Utils
{
	public static class MirageSDKHelper
	{
		public static string StringToBigInteger(string value)
		{
			var bnValue = BigInteger.Parse(value);
			return "0x" + bnValue.ToString("X");
		}

		public static UnityWebRequest GetUnityWebRequestFromJSON(string url, string json)
		{
			var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
			var bytes = Encoding.UTF8.GetBytes(json);
			var uH = new UploadHandlerRaw(bytes);
			request.uploadHandler = uH;
			request.SetRequestHeader("Content-Type", "application/json");
			request.downloadHandler = new DownloadHandlerBuffer();
			return request;
		}
	}
}