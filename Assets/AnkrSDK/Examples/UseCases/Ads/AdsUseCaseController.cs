using AnkrAds.Ads.Data;
using AnkrSDK.Ads.UI;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Examples.ERC20Example;
using AnkrSDK.Provider;
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

		private IEthHandler _eth;

		public override void ActivateUseCase()
		{
			base.ActivateUseCase();

			var ankrSDK = AnkrSDKFactory.GetAnkrSDKInstance(ERC20ContractInformation.HttpProviderURL);
			_eth = ankrSDK.Eth;
		}

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

			var defaultAccount = await _eth.GetDefaultAccount();
			var requestResult = await AnkrAds.Ads.AnkrAds.DownloadAdData(AdType.Banner, defaultAccount);

			if (requestResult != null)
			{
				await UniTask.WhenAll(
					_ankrBannerAdImage.SetupAd(requestResult),
					_ankrBannerAdSprite.SetupAd(requestResult));
			}

			_ankrBannerAdImage.TryShow();
			_ankrBannerAdSprite.TryShow();
		}

		public override void DeActivateUseCase()
		{
			base.DeActivateUseCase();
			_ankrBannerAdImage.gameObject.SetActive(false);
			_ankrBannerAdSprite.gameObject.SetActive(false);
		}
	}
}