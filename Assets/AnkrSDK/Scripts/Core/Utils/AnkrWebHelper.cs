using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace AnkrSDK.Core.Utils
{
	public static class AnkrWebHelper
	{
		public static async UniTask<Sprite> GetImageFromURL(string uri)
		{
			using (var webRequest = UnityWebRequestTexture.GetTexture(uri))
			{
				await webRequest.SendWebRequest();
				if (webRequest.isNetworkError || webRequest.isHttpError)
				{
					Debug.LogError("Network error while getting " + uri);
					return null;
				}

				var result = DownloadHandlerTexture.GetContent(webRequest);
				var sprite = Sprite.Create(result, new Rect(0, 0, result.width, result.height),
					new Vector2(0, 0));
				sprite.name = uri;
				return sprite;
			}
		}
	}
}