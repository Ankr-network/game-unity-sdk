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

		public static UniTask<AdRequestResult> RequestAdData(string accountAddress, AdType adType)
		{
			return MakeRequest(accountAddress, adType);
		}

		private static async UniTask<AdRequestResult> MakeRequest(string accountAddress, AdType adType)
		{
			return await SendAdRequest(accountAddress);
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
					var result = JsonConvert.DeserializeObject<AdRequestResult>(json);
					if (result.Result)
					{
						Debug.Log($"Received Ad Data:{json}");
						return result;
					}

					Debug.LogError("Error while making an ad request");
					return null;
				}

				Debug.LogError(www.error);
				return null;
			}
		}
	}
}