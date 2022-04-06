using AnkrSDK.Ads.Data;
using AnkrSDK.Core.Utils;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Ads
{
	public static class AnkrAds
	{
		private const string RandomImageURL = "https://picsum.photos/300/300";

		public static UniTask<AdRequestResult> RequestAdData(string accountAddress, AdType adType)
		{
			return MakeRequest(accountAddress, adType);
		}

		private static async UniTask<AdRequestResult> MakeRequest(string accountAddress, AdType adType)
		{
			var result = await AnkrWebHelper.GetImageFromURL(RandomImageURL);
			var adData = new AdData
			{
				Size = result.rect.size,
				Sprite = result,
				URL = RandomImageURL
			};
			return new AdRequestResult
			{
				AdData = adData
			};
		}
	}
}