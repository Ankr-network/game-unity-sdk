using UnityEngine;

namespace AnkrSDK.UseCases.Ads
{
	[RequireComponent(typeof(AdsCallbackListener))]
	public class AdsUseCaseController : UseCase
	{
		private const string FullscreenAdTestUnitId = "2592a670-f7ab-4400-ab95-af7d5975a9f8";
		private const string BannerAdTestUnitId = "cc758a98-bb29-4628-89c2-4fa5294ccf28";
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
			const string testAppId = "9e6624ba-0653-4230-86b8-204bddca8a8f";
			_adsCallbackListener.UnsubscribeToCallbackListenerEvents();
			_adsCallbackListener.SubscribeToCallbackListenerEvents();

			AnkrAds.Ads.AnkrAds.Initialize(testAppId, walletAddress, RuntimePlatform.Android);
		}

		private void OnLoadFullscreenAdButtonClick()
		{
			AnkrAds.Ads.AnkrAds.LoadAd(FullscreenAdTestUnitId);
		}

		private void OnLoadImageButtonClick()
		{
			AnkrAds.Ads.AnkrAds.LoadAdTexture(BannerAdTestUnitId);
		}

		private void OnViewButtonClick()
		{
			AnkrAds.Ads.AnkrAds.ShowAd(FullscreenAdTestUnitId);
		}
	}
}