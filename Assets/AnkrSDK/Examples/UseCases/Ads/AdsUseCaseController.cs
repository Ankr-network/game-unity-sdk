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
		[SerializeField] private Button _loadButton;
		[SerializeField] private Button _viewButton;
		[SerializeField] private AnkrBannerAdImage _ankrBannerAdImage;
		[SerializeField] private AnkrBannerAdSprite _ankrBannerAdSprite;
		[SerializeField] private TMP_Text _logs;

		public override void DeActivateUseCase()
		{
			base.DeActivateUseCase();
			_ankrBannerAdImage.gameObject.SetActive(false);
			_ankrBannerAdSprite.gameObject.SetActive(false);
		}
		
		private void SubscribeToCallbackListenerEvents()
		{
			AnkrAds.Ads.AnkrAds.OnAdInitialized += CallbackListenerOnAdInitialized;
			AnkrAds.Ads.AnkrAds.OnAdClicked += CallbackListenerOnAdClicked;
			AnkrAds.Ads.AnkrAds.OnAdClosed += CallbackListenerOnAdClosed;
			AnkrAds.Ads.AnkrAds.OnAdFinished += CallbackListenerOnAdFinished;
			AnkrAds.Ads.AnkrAds.OnAdLoaded += CallbackListenerOnAdLoaded;
			AnkrAds.Ads.AnkrAds.OnAdOpened += CallbackListenerOnAdOpened;
			AnkrAds.Ads.AnkrAds.OnAdFailedToLoad += CallbackListenerOnAdFailedToLoad;
			AnkrAds.Ads.AnkrAds.OnAdTextureReceived += CallbackListenerOnAdTextureReceived;
		}
		
		private void UnsubscribeToCallbackListenerEvents()
		{
			AnkrAds.Ads.AnkrAds.OnAdInitialized -= CallbackListenerOnAdInitialized;
			AnkrAds.Ads.AnkrAds.OnAdClicked -= CallbackListenerOnAdClicked;
			AnkrAds.Ads.AnkrAds.OnAdClosed -= CallbackListenerOnAdClosed;
			AnkrAds.Ads.AnkrAds.OnAdFinished -= CallbackListenerOnAdFinished;
			AnkrAds.Ads.AnkrAds.OnAdLoaded -= CallbackListenerOnAdLoaded;
			AnkrAds.Ads.AnkrAds.OnAdOpened -= CallbackListenerOnAdOpened;
			AnkrAds.Ads.AnkrAds.OnAdFailedToLoad -= CallbackListenerOnAdFailedToLoad;
			AnkrAds.Ads.AnkrAds.OnAdTextureReceived -= CallbackListenerOnAdTextureReceived;
		}
		
		private async void CallbackListenerOnAdTextureReceived(string unitID, byte[] adTextureData)
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdTextureReceived");
			
			var texture = new Texture2D(2,2);
			texture.LoadImage(adTextureData);

			_ankrBannerAdImage.SetupAd(texture);
			_ankrBannerAdSprite.SetupAd(texture);

			_ankrBannerAdImage.TryShow();
			_ankrBannerAdSprite.TryShow();
			
			ActivateNextButton(1);
		}

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
			ActivateNextButton(2);
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

		private void OnInitializeButtonClick()
		{
			const string walletAddress = "This is ankr mobile address";
			const string appId = "1c562170-9ee5-4157-a5f8-e99c32e73cb0";
			UnsubscribeToCallbackListenerEvents();
			SubscribeToCallbackListenerEvents();

			AnkrAds.Ads.AnkrAds.Initialize(appId, walletAddress, RuntimePlatform.WindowsEditor);
		}

		private void OnLoadButtonClick()
		{
			const string testUnitId = "d396af2c-aa3a-44da-ba17-68dbb7a8daa1";
			AnkrAds.Ads.AnkrAds.LoadAdTexture(testUnitId);
		}

		private void OnViewButtonClick()
		{
			const string testUnitId = "d396af2c-aa3a-44da-ba17-68dbb7a8daa1";
			AnkrAds.Ads.AnkrAds.ShowAd(testUnitId);
		}
		
		private void OnEnable()
		{
			_ankrBannerAdImage.gameObject.SetActive(false);
			_ankrBannerAdSprite.gameObject.SetActive(false);
			ActivateNextButton(0);

			_initializeButton.onClick.AddListener(OnInitializeButtonClick);
			_loadButton.onClick.AddListener(OnLoadButtonClick);
			_viewButton.onClick.AddListener(OnViewButtonClick);
		}

		private void OnDisable()
		{
			_initializeButton.onClick.RemoveAllListeners();
			_viewButton.onClick.RemoveAllListeners();
			_loadButton.onClick.RemoveAllListeners();
			UnsubscribeToCallbackListenerEvents();
		}

		private void ActivateNextButton(int buttonToActivate)
		{
			_initializeButton.interactable = buttonToActivate == 0;
			_loadButton.interactable = buttonToActivate == 1;
			_viewButton.interactable = buttonToActivate == 2;
		}

		private void UpdateUILogs(string log)
		{
			_logs.text += "\n" + log;
			Debug.Log(log);
		}
	}
}