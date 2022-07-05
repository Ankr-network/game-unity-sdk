using AnkrSDK.Ads.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UseCases.Ads
{
	public class AdsUseCaseController : UseCase
	{
		[SerializeField] private Button _initializeButton;
		[SerializeField] private Button _loadFullscreenAdButton;
		[SerializeField] private Button _loadBannerAdButton;
		[SerializeField] private Button _viewButton;
		[SerializeField] private AnkrBannerAdImage _ankrBannerAdImage;
		[SerializeField] private AnkrBannerAdSprite _ankrBannerAdSprite;
		[SerializeField] private TMP_Text _logs;

		private const string FullscreenAdTestUnitId = "2592a670-f7ab-4400-ab95-af7d5975a9f8";
		private const string BannerAdTestUnitId = "cc758a98-bb29-4628-89c2-4fa5294ccf28";

		public override void DeActivateUseCase()
		{
			base.DeActivateUseCase();
			_ankrBannerAdImage.gameObject.SetActive(false);
			_ankrBannerAdSprite.gameObject.SetActive(false);
		}

		#region Subscriptions

		private void SubscribeToCallbackListenerEvents()
		{
			AnkrAds.Ads.AnkrAds.OnAdInitialized += CallbackListenerOnAdInitialized;
			AnkrAds.Ads.AnkrAds.OnAdClicked += CallbackListenerOnAdClicked;
			AnkrAds.Ads.AnkrAds.OnAdClosed += CallbackListenerOnAdClosed;
			AnkrAds.Ads.AnkrAds.OnAdFinished += CallbackListenerOnAdFinished;
			AnkrAds.Ads.AnkrAds.OnAdLoaded += CallbackListenerOnAdLoaded;
			AnkrAds.Ads.AnkrAds.OnAdOpened += CallbackListenerOnAdOpened;
			AnkrAds.Ads.AnkrAds.OnAdRewarded += CallbackListenerOnAdRewarded;
			AnkrAds.Ads.AnkrAds.OnAdFailedToLoad += CallbackListenerOnAdFailedToLoad;
			AnkrAds.Ads.AnkrAds.OnAdTextureReceived += CallbackListenerOnAdTextureReceived;
			AnkrAds.Ads.AnkrAds.OnError += CallbackListenerOnError;
		}

		private void UnsubscribeToCallbackListenerEvents()
		{
			AnkrAds.Ads.AnkrAds.OnAdInitialized -= CallbackListenerOnAdInitialized;
			AnkrAds.Ads.AnkrAds.OnAdClicked -= CallbackListenerOnAdClicked;
			AnkrAds.Ads.AnkrAds.OnAdClosed -= CallbackListenerOnAdClosed;
			AnkrAds.Ads.AnkrAds.OnAdFinished -= CallbackListenerOnAdFinished;
			AnkrAds.Ads.AnkrAds.OnAdLoaded -= CallbackListenerOnAdLoaded;
			AnkrAds.Ads.AnkrAds.OnAdOpened -= CallbackListenerOnAdOpened;
			AnkrAds.Ads.AnkrAds.OnAdRewarded -= CallbackListenerOnAdRewarded;
			AnkrAds.Ads.AnkrAds.OnAdFailedToLoad -= CallbackListenerOnAdFailedToLoad;
			AnkrAds.Ads.AnkrAds.OnAdTextureReceived -= CallbackListenerOnAdTextureReceived;
			AnkrAds.Ads.AnkrAds.OnError -= CallbackListenerOnError;
		}

		#endregion

		#region CallbackListener

		private async void CallbackListenerOnAdInitialized()
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdInitialized");
			ActivateNextButton(1);
		}

		private async void CallbackListenerOnAdLoaded(string uuid)
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdLoaded");

			if (uuid == FullscreenAdTestUnitId)
			{
				ActivateNextButton(2);
			}
		}

		private async void CallbackListenerOnAdClicked(string uuid)
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdClicked");
		}

		private async void CallbackListenerOnAdClosed()
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdClosed");
		}

		private async void CallbackListenerOnAdFinished()
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdFinished");
			ActivateNextButton(1);
		}

		private async void CallbackListenerOnAdRewarded(string uuid)
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdRewarded");
		}

		private async void CallbackListenerOnAdOpened()
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdOpened");
		}

		private async void CallbackListenerOnAdFailedToLoad(string uuid)
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdFailedToLoad");
		}

		private async void CallbackListenerOnError(string errorMessage)
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("Error : " + errorMessage);
		}

		private async void CallbackListenerOnAdTextureReceived(string unitID, byte[] adTextureData)
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdTextureReceived");

			var texture = new Texture2D(2, 2);
			texture.LoadImage(adTextureData);

			_ankrBannerAdImage.SetupAd(texture);
			_ankrBannerAdSprite.SetupAd(texture);

			_ankrBannerAdImage.TryShow();
			_ankrBannerAdSprite.TryShow();

			ActivateNextButton(1);
		}

		#endregion

		#region OnButtonClicks

		private void OnInitializeButtonClick()
		{
			const string walletAddress = "This is ankr mobile address";
			const string testAppId = "9e6624ba-0653-4230-86b8-204bddca8a8f";
			UnsubscribeToCallbackListenerEvents();
			SubscribeToCallbackListenerEvents();

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

		#endregion

		private void OnEnable()
		{
			_ankrBannerAdImage.gameObject.SetActive(false);
			_ankrBannerAdSprite.gameObject.SetActive(false);
			ActivateNextButton(0);

			_initializeButton.onClick.AddListener(OnInitializeButtonClick);
			_loadBannerAdButton.onClick.AddListener(OnLoadImageButtonClick);
			_loadFullscreenAdButton.onClick.AddListener(OnLoadFullscreenAdButtonClick);
			_viewButton.onClick.AddListener(OnViewButtonClick);
		}

		private void OnDisable()
		{
			_initializeButton.onClick.RemoveAllListeners();
			_viewButton.onClick.RemoveAllListeners();
			_loadBannerAdButton.onClick.RemoveAllListeners();
			_loadFullscreenAdButton.onClick.RemoveAllListeners();
			UnsubscribeToCallbackListenerEvents();
		}

		private void ActivateNextButton(int buttonToActivate)
		{
			_initializeButton.interactable = buttonToActivate == 0;
			_loadBannerAdButton.interactable = buttonToActivate == 1;
			_loadFullscreenAdButton.interactable = buttonToActivate == 1;
			_viewButton.interactable = buttonToActivate == 2;
		}

		private void UpdateUILogs(string log)
		{
			_logs.text += "\n" + log;
			Debug.Log(log);
		}
	}
}