using AnkrSDK.Base;
using UnityEngine;

namespace AnkrSDK.Ads.Examples
{
	public class AdsUseCaseController : UseCaseBodyUI
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

		public override void SetUseCaseBodyActive(bool active)
		{
			base.SetUseCaseBodyActive(active);
			_adsCallbackListener.ActivateBillboardAds(false);
		}

		private void OnInitializeButtonClick()
		{
			const string walletAddress = "This is ankr mobile address";
			_adsCallbackListener.UnsubscribeToCallbackListenerEvents();
			_adsCallbackListener.SubscribeToCallbackListenerEvents();

			AnkrAdvertisements.Initialize(AdsBackendInformation.TestAppId, walletAddress);
		}

		private void OnLoadFullscreenAdButtonClick()
		{
			AnkrAdvertisements.LoadAd(AdsBackendInformation.FullscreenAdTestUnitId);
		}

		private void OnLoadImageButtonClick()
		{
			AnkrAdvertisements.LoadAdTexture(AdsBackendInformation.BannerAdTestUnitId);
		}

		private void OnViewButtonClick()
		{
			AnkrAdvertisements.ShowAd(AdsBackendInformation.FullscreenAdTestUnitId);
		}
	}
}