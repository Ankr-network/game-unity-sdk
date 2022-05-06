using AnkrAds.Ads.Data;
using AnkrSDK.Ads.UI;
using AnkrSDK.Core;
using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Core.Utils;
using AnkrSDK.Examples.ERC20Example;
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

		private bool _isInitialized = false;

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
			
			var requestResult = await AnkrAds.Ads.AnkrAds.DownloadAdData(AdType.Banner, await _eth.GetDefaultAccount());

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