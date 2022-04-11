using AnkrSDK.Ads;
using AnkrSDK.Ads.Data;
using AnkrSDK.Ads.UI;
using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UseCases.Ads
{
	public class AdsUseCaseController : UseCase
	{
		[SerializeField] private Button _button;
		[SerializeField] private AnkrBannerAdImage _ankrBannerAdImage;
		[SerializeField] private AnkrBannerAdSprite _ankrBannerAdSprite;

		private void OnEnable()
		{
			_ankrBannerAdImage.gameObject.SetActive(false);
			_ankrBannerAdSprite.gameObject.SetActive(false);
			_button.gameObject.SetActive(true);
			_button.onClick.AddListener(OnButtonClick);
		}

		private void OnDisable()
		{
			_button.onClick.RemoveAllListeners();
		}

		private void OnButtonClick()
		{
			DownloadAd().Forget();
		}

		private async UniTaskVoid DownloadAd()
		{
			_button.gameObject.SetActive(false);

			var requestResult = await AnkrAds.RequestAdData(EthHandler.DefaultAccount, AdType.Banner);

			if (requestResult == null)
			{
				return;
			}

			var data = requestResult;
			var sprite = await AnkrWebHelper.GetImageFromURL(requestResult.TextureURL);
			_ankrBannerAdImage.SetupAd(sprite);
			_ankrBannerAdSprite.SetupAd(sprite);
			_ankrBannerAdImage.gameObject.SetActive(true);
			_ankrBannerAdSprite.gameObject.SetActive(true);
		}
	}
}