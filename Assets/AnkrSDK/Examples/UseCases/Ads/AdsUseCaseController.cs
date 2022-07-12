using UnityEngine;

namespace AnkrSDK.UseCases.Ads
{
	public class AdsUseCaseController : UseCase
	{
		[SerializeField] private AdsCallbackListener _adsCallbackListener;

		private void OnEnable()
		{
			_adsCallbackListener.ActivateBillboardAds(false);

			_adsCallbackListener.GetInitializeButton().onClick.AddListener(OnInitializeButtonClick);
			_adsCallbackListener.GetLoadBannerAdButton().onClick.AddListener(OnLoadImageButtonClick);
			_adsCallbackListener.GetLoadFullscreenAdButton().onClick.AddListener(OnLoadFullscreenAdButtonClick);
			_adsCallbackListener.GetViewButton().onClick.AddListener(OnViewButtonClick);
		}

		private void OnDisable()
		{
			_adsCallbackListener.GetInitializeButton().onClick.RemoveAllListeners();
			_adsCallbackListener.GetLoadBannerAdButton().onClick.RemoveAllListeners();
			_adsCallbackListener.GetLoadFullscreenAdButton().onClick.RemoveAllListeners();
			_adsCallbackListener.GetViewButton().onClick.RemoveAllListeners();
			_adsCallbackListener.UnsubscribeToCallbackListenerEvents();
		}

		public override void DeActivateUseCase()
		{
			base.DeActivateUseCase();
			_adsCallbackListener.ActivateBillboardAds(false);
		}

		private void OnInitializeButtonClick()
		{
			const string walletAddress = "This is ankr mobile address";
			_adsCallbackListener.UnsubscribeToCallbackListenerEvents();
			_adsCallbackListener.SubscribeToCallbackListenerEvents();

			AnkrAds.Ads.AnkrAds.Initialize(AdsBackendInformation.TestAppId, walletAddress, Application.platform);
		}

		private void OnLoadFullscreenAdButtonClick()
		{
			AnkrAds.Ads.AnkrAds.LoadAd(AdsBackendInformation.FullscreenAdTestUnitId);
		}

		private void OnLoadImageButtonClick()
		{
			AnkrAds.Ads.AnkrAds.LoadAdTexture(AdsBackendInformation.BannerAdTestUnitId);
		}

		private void OnViewButtonClick()
		{
			AnkrAds.Ads.AnkrAds.ShowAd(AdsBackendInformation.FullscreenAdTestUnitId);
		}
	}
}