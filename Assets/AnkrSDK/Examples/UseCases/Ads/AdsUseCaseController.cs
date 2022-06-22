using AnkrAds.Ads;
using AnkrAds.Ads.Data;
using AnkrSDK.Ads;
using AnkrSDK.Ads.UI;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Examples.ERC20Example;
using AnkrSDK.Provider;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UseCases.Ads
{
	public class AdsUseCaseController : UseCase
	{
		[SerializeField] private Button _getTextureButton;
		[SerializeField] private Button _initializeButton;
		[SerializeField] private Button _loadFullscreenAdButton;
		[SerializeField] private Button _loadImageButton;
		[SerializeField] private Button _viewButton;
		[SerializeField] private AnkrBannerAdImage _ankrBannerAdImage;
		[SerializeField] private AnkrBannerAdSprite _ankrBannerAdSprite;
		[SerializeField] private TMP_Text _logs;

		private IEthHandler _eth;

		private AnkrAdsAndroidCallbackListener _ankrAdsAndroidCallbackListener;
		
		private const string FullscreenAdTestUnitId = "test_video";
		private const string ImageTestUnitId = "d396af2c-aa3a-44da-ba17-68dbb7a8daa1";

		#region UseCase Override
		public override void ActivateUseCase()
		{
			base.ActivateUseCase();

			var ankrSDK = AnkrSDKFactory.GetAnkrSDKInstance(ERC20ContractInformation.HttpProviderURL);
			_eth = ankrSDK.Eth;
		}
		
		public override void DeActivateUseCase()
		{
			base.DeActivateUseCase();
			_ankrBannerAdImage.gameObject.SetActive(false);
			_ankrBannerAdSprite.gameObject.SetActive(false);
		}
		#endregion
		
		#region Event Subscription
		private void SubscribeToCallbackListenerEvents()
		{
			_ankrAdsAndroidCallbackListener.OnAdInitialized += CallbackListenerOnAdInitialized;
			_ankrAdsAndroidCallbackListener.OnAdClicked += CallbackListenerOnAdClicked;
			_ankrAdsAndroidCallbackListener.OnAdClosed += CallbackListenerOnAdClosed;
			_ankrAdsAndroidCallbackListener.OnAdFinished += CallbackListenerOnAdFinished;
			_ankrAdsAndroidCallbackListener.OnAdLoaded += CallbackListenerOnAdLoaded;
			_ankrAdsAndroidCallbackListener.OnAdOpened += CallbackListenerOnAdOpened;
			_ankrAdsAndroidCallbackListener.OnAdFailedToLoad += CallbackListenerOnAdFailedToLoad;
			_ankrAdsAndroidCallbackListener.OnAdTextureReceived += CallbackListenerOnAdTextureReceived;
		}
		
		private void UnsubscribeToCallbackListenerEvents()
		{
			_ankrAdsAndroidCallbackListener.OnAdInitialized -= CallbackListenerOnAdInitialized;
			_ankrAdsAndroidCallbackListener.OnAdClicked -= CallbackListenerOnAdClicked;
			_ankrAdsAndroidCallbackListener.OnAdClosed -= CallbackListenerOnAdClosed;
			_ankrAdsAndroidCallbackListener.OnAdFinished -= CallbackListenerOnAdFinished;
			_ankrAdsAndroidCallbackListener.OnAdLoaded -= CallbackListenerOnAdLoaded;
			_ankrAdsAndroidCallbackListener.OnAdOpened -= CallbackListenerOnAdOpened;
			_ankrAdsAndroidCallbackListener.OnAdFailedToLoad -= CallbackListenerOnAdFailedToLoad;
			_ankrAdsAndroidCallbackListener.OnAdTextureReceived -= CallbackListenerOnAdTextureReceived;
		}
		#endregion
		
		#region Callback Listener Events
		private async void CallbackListenerOnAdTextureReceived(string unitID, byte[] adTextureData)
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdTextureReceived");
			
			Texture2D texture = new Texture2D(2,2);
			texture.LoadImage(adTextureData);
			
			await UniTask.WhenAll(
				_ankrBannerAdImage.SetupAd(texture),
				_ankrBannerAdSprite.SetupAd(texture));

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

		private async void CallbackListenerOnAdLoaded(string uuid)
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdLoaded " + uuid);
			ActivateNextButton(2);
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
		#endregion

		#region On Button Clicks
		private void OnInitializeButtonClick()
		{
			const string walletAddress = "This is ankr mobile address";
			const string appId = "1c562170-9ee5-4157-a5f8-e99c32e73cb0";
			_ankrAdsAndroidCallbackListener = new AnkrAdsAndroidCallbackListener();
			UnsubscribeToCallbackListenerEvents();
			SubscribeToCallbackListenerEvents();
			AnkrAdsNativeAndroid.Initialize(appId, walletAddress,_ankrAdsAndroidCallbackListener);
		}

		private void OnLoadFullscreenAdButtonClick()
		{
			AnkrAdsNativeAndroid.LoadAd(FullscreenAdTestUnitId);
		}
		
		private void OnLoadImageButtonClick()
		{
			AnkrAdsNativeAndroid.LoadAdTexture(ImageTestUnitId);
		}
		
		private void OnGetTextureClick()
		{
			AnkrAdsNativeAndroid.GetTextureByteArray(ImageTestUnitId);
		}

		private void OnViewButtonClick()
		{
			AnkrAdsNativeAndroid.ShowAd(FullscreenAdTestUnitId);
		}
		#endregion
		
		private void OnEnable()
		{
			_ankrBannerAdImage.gameObject.SetActive(false);
			_ankrBannerAdSprite.gameObject.SetActive(false);

			_initializeButton.interactable = true;
			_viewButton.interactable = false;
			_loadFullscreenAdButton.interactable = false;
			_loadImageButton.interactable = false;
			_getTextureButton.interactable = false;

			_initializeButton.onClick.AddListener(OnInitializeButtonClick);
			_loadImageButton.onClick.AddListener(OnLoadImageButtonClick);
			_loadFullscreenAdButton.onClick.AddListener(OnLoadFullscreenAdButtonClick);
			_viewButton.onClick.AddListener(OnViewButtonClick);
			_getTextureButton.onClick.AddListener(OnGetTextureClick);
		}

		private void OnDisable()
		{
			_initializeButton.onClick.RemoveAllListeners();
			_viewButton.onClick.RemoveAllListeners();
			_loadFullscreenAdButton.onClick.RemoveAllListeners();
			_loadImageButton.onClick.RemoveAllListeners();
			UnsubscribeToCallbackListenerEvents();
		}

		private void ActivateNextButton(int buttonToActivate)
		{
			_initializeButton.interactable = buttonToActivate == 0;
			_loadFullscreenAdButton.interactable = buttonToActivate == 1;
			_loadImageButton.interactable = buttonToActivate == 1;
			_viewButton.interactable = buttonToActivate == 2;
			_getTextureButton.interactable = buttonToActivate == 2;
		}

		private void UpdateUILogs(string log)
		{
			_logs.text += "\n" + log;
			Debug.Log(log);
		}
	}
}