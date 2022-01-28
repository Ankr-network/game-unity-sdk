using System.Text;
using UnityEngine.Networking;

namespace Utility.Utils
{
	public class WebRequests
	{
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

		protected static byte[] GetBytes(string str)
		{
			var bytes = Encoding.UTF8.GetBytes(str);
			return bytes;
		}
	}
}