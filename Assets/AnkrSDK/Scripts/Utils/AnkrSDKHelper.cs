using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Web;
using UnityEngine.Networking;

namespace AnkrSDK.Utils
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
			var bytes = Encoding.UTF8.GetBytes(json);
			var uH = new UploadHandlerRaw(bytes);
			request.uploadHandler = uH;
			request.SetRequestHeader("Content-Type", "application/json");
			request.downloadHandler = new DownloadHandlerBuffer();
			return request;
		}
		
		private static string ToQueryString(Dictionary<string, string[]> queryDictionary)
		{
			var array = (
				from key in queryDictionary.Keys
				from value in queryDictionary[key]
				select string.Format(
					"{0}={1}",
					HttpUtility.UrlEncode(key),
					HttpUtility.UrlEncode(value))
			).ToArray();
			return "?" + string.Join("&", array);
		}
	}
}