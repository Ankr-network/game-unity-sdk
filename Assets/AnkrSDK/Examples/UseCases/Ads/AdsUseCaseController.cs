using AnkrSDK.Ads;
using AnkrSDK.Ads.Data;
using AnkrSDK.Ads.UI;
using AnkrSDK.Core.Implementation;
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
			var requestResult = await AnkrAds.RequestAdData(EthHandler.DefaultAccount, AdType.Banner);

			_button.gameObject.SetActive(false);
			var resultError = requestResult.Error;
			if (!string.IsNullOrEmpty(resultError))
			{
				Debug.LogError(resultError);
				return;
			}

			var data = requestResult.AdData;
			_ankrBannerAdImage.SetupAd(data);
			_ankrBannerAdSprite.SetupAd(data);
			_ankrBannerAdImage.gameObject.SetActive(true);
			_ankrBannerAdSprite.gameObject.SetActive(true);
		}
	}
}