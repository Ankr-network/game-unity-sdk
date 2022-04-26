using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AnkrSDK.Ads
{
	public static class AdsWebHelper
	{
		public static async UniTask<TResultType> SendPostRequest<TResultType>(string url, WWWForm form)
		{
			using (var www = UnityWebRequest.Post(url, form))
			{
				await www.SendWebRequest();

				var json = www.downloadHandler.text;
				if (www.result == UnityWebRequest.Result.Success)
				{
					try
					{
						var result = JsonConvert.DeserializeObject<TResultType>(json);
						return result;
					}
					catch (Exception e)
					{
						Debug.LogError(e.Message);
						return default;
					}
				}

				Debug.LogError(www.error);
				return default;
			}
		}
		
		public static string AppLanguage()
		{
			switch (Application.systemLanguage)
			{
				case SystemLanguage.English: return "EN";
				case SystemLanguage.French: return "FR";
				case SystemLanguage.Spanish: return "ES";
				case SystemLanguage.Chinese: return "CN";
			}

			return null;
		}
	}
}