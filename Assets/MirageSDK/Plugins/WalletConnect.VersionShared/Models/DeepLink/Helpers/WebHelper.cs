using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MirageSDK.WalletConnect.VersionShared.Models.DeepLink.Helpers
{
	public static class WebHelper
	{
		public static async UniTask<Texture2D> GetTextureFromGenericUri(string genericUri)
		{
			if (genericUri.StartsWith("resources:"))
			{
				var indexOfColor = genericUri.IndexOf(":", StringComparison.InvariantCulture);
				if (indexOfColor >= genericUri.Length - 1)
				{
					throw new InvalidOperationException($"Invalid URI format for " + genericUri);
				}

				var resourcesPath = genericUri.Split(":")[1];

				var request = Resources.LoadAsync<Texture2D>(resourcesPath);
				var result = await request.ToUniTask();
				var texture = result as Texture2D;

				if (texture == null)
				{
					if (result == null)
					{
						Debug.LogError("Texture not found at " + genericUri);
					}
					else
					{
						Debug.LogError("Object at " + genericUri + " is not a texture");
					}
				}
				
				return texture;
			}
			
			using (var imageRequest = UnityWebRequestTexture.GetTexture(genericUri))
			{
				try
				{
					await imageRequest.SendWebRequest();
				}
				catch (UnityWebRequestException e)
				{
					var downloadHandler = ((DownloadHandlerTexture)imageRequest.downloadHandler);
					Debug.LogError(
						$"Exception while loading texture {e.Message} download handler error: {downloadHandler.error}");
					return null;
				}

				#if UNITY_2020_2_OR_NEWER
				if (imageRequest.result != UnityWebRequest.Result.Success)
				{
					Debug.Log("Error Getting Wallet Icon: " + imageRequest.error);
				}
				#else
                    if (imageRequest.isHttpError || imageRequest.isNetworkError)
                    {
                        Debug.Log("Error Getting Wallet Icon: " + imageRequest.error);
                    }
				#endif
				else
				{
					var downloadHandler = ((DownloadHandlerTexture)imageRequest.downloadHandler);
					var texture = downloadHandler.texture;
					return texture;
				}
			}

			return null;
		}
	}
}