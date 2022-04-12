using System;
using AnkrSDK.Ads.Data;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AnkrSDK.Ads
{
	public static class AnkrAds
	{
		private const string PublicAddress = "public_address";

		private const string RandomImageURLBase = "http://45.77.189.28:5001/ad";

		public static async UniTask<AdData> RequestAdData(string accountAddress, AdType adType)
		{
			var adRequestResult = await SendAdRequest(accountAddress);
			return string.IsNullOrEmpty(adRequestResult.Error)
				? adRequestResult.AdData
				: null;
		}

		private static async UniTask<AdRequestResult> SendAdRequest(string accountAddress)
		{
			var form = new WWWForm();
			form.AddField(PublicAddress, accountAddress);

			using (var www = UnityWebRequest.Post(RandomImageURLBase, form))
			{
				await www.SendWebRequest();

				var json = www.downloadHandler.text;

				if (www.result == UnityWebRequest.Result.Success)
				{
					try
					{
						var result = JsonConvert.DeserializeObject<AdRequestResult>(json);
						return result;
					}
					catch (Exception e)
					{
						Debug.LogError(e.Message);
						return null;
					}
				}

				Debug.LogError(www.error);
				return null;
			}
		}
	}
}