using AnkrAds.Ads;
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
		[SerializeField] private Button _initializeButton;
		[SerializeField] private Button _loadButton;
		[SerializeField] private Button _viewButton;
		[SerializeField] private AnkrBannerAdImage _ankrBannerAdImage;
		[SerializeField] private AnkrBannerAdSprite _ankrBannerAdSprite;
		[SerializeField] private TMP_Text _logs;

		private IEthHandler _eth;

		private void OnEnable()
		{
			_ankrBannerAdImage.gameObject.SetActive(false);
			_ankrBannerAdSprite.gameObject.SetActive(false);

			_initializeButton.interactable = true;
			_viewButton.interactable = false;
			_loadButton.interactable = false;

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

		public override void ActivateUseCase()
		{
			base.ActivateUseCase();

			var ankrSDK = AnkrSDKFactory.GetAnkrSDKInstance(ERC20ContractInformation.HttpProviderURL);
			_eth = ankrSDK.Eth;
		}

		private void ActivateNextButton(int buttonToActivate)
		{
			switch (buttonToActivate)
			{
				case 0:
					_initializeButton.interactable = true;
					_viewButton.interactable = false;
					_loadButton.interactable = false;
					break;
				case 1:
					_initializeButton.interactable = false;
					_viewButton.interactable = false;
					_loadButton.interactable = true;
					break;
				case 2:
					_initializeButton.interactable = false;
					_viewButton.interactable = true;
					_loadButton.interactable = false;
					break;
				default:
					Debug.LogError("WrongButtonNb");
					break;
			}
		}

		private void SubscribeToCallbackListenerEvents()
		{
			AnkrAdsNativeAndroid.callbackListener.OnAdInitialized += CallbackListenerOnAdInitialized;
			AnkrAdsNativeAndroid.callbackListener.OnAdClicked += CallbackListenerOnAdClicked;
			AnkrAdsNativeAndroid.callbackListener.OnAdClosed += CallbackListenerOnAdClosed;
			AnkrAdsNativeAndroid.callbackListener.OnAdFinished += CallbackListenerOnAdFinished;
			AnkrAdsNativeAndroid.callbackListener.OnAdLoaded += CallbackListenerOnAdLoaded;
			AnkrAdsNativeAndroid.callbackListener.OnAdOpened += CallbackListenerOnAdOpened;
			AnkrAdsNativeAndroid.callbackListener.OnAdFailedToLoad += CallbackListenerOnAdFailedToLoad;
		}

		private void UpdateUILogs(string log)
		{
			_logs.text += "\n" + log;
			Debug.Log(log);
		}

		private async void CallbackListenerOnAdInitialized()
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdInitialized");
			ActivateNextButton(1);
		}

		private async void CallbackListenerOnAdClicked()
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
			ActivateNextButton(3);
		}

		private async void CallbackListenerOnAdLoaded()
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdLoaded");
			ActivateNextButton(2);
		}

		private async void CallbackListenerOnAdOpened()
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdOpened");
		}

		private async void CallbackListenerOnAdFailedToLoad()
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdFailedToLoad");
		}

		private void UnsubscribeToCallbackListenerEvents()
		{
			AnkrAdsNativeAndroid.callbackListener.OnAdClicked -= CallbackListenerOnAdLoaded;
			AnkrAdsNativeAndroid.callbackListener.OnAdClosed -= CallbackListenerOnAdLoaded;
			AnkrAdsNativeAndroid.callbackListener.OnAdFinished -= CallbackListenerOnAdLoaded;
			AnkrAdsNativeAndroid.callbackListener.OnAdLoaded -= CallbackListenerOnAdLoaded;
			AnkrAdsNativeAndroid.callbackListener.OnAdOpened -= CallbackListenerOnAdLoaded;
			AnkrAdsNativeAndroid.callbackListener.OnAdFailedToLoad -= CallbackListenerOnAdLoaded;
		}

		private void OnInitializeButtonClick()
		{
			const string walletAddress = "This is ankr mobile address";
			const string appId = "1c562170-9ee5-4157-a5f8-e99c32e73cb0";
			AnkrAdsNativeAndroid.SetNewAdsCallbackListener();
			UnsubscribeToCallbackListenerEvents();
			SubscribeToCallbackListenerEvents();
			AnkrAdsNativeAndroid.Initialize(appId, walletAddress);
		}

		private void OnLoadButtonClick()
		{
			const string unitId = "d396af2c-aa3a-44da-ba17-68dbb7a8daa1";
			AnkrAdsNativeAndroid.LoadAd(unitId);
		}

		private void OnViewButtonClick()
		{
			const string unitId = "d396af2c-aa3a-44da-ba17-68dbb7a8daa1";
			AnkrAdsNativeAndroid.ShowAd(unitId);
		}

		private async UniTaskVoid DownloadAd()
		{
			//_button.gameObject.SetActive(false);

			var defaultAccount = await _eth.GetDefaultAccount();
			var testAppId = "e8d0f552-22a5-482c-a149-2d51bace6ccb";
			var testUnitId = "d396af2c-aa3a-44da-ba17-68dbb7a8daa1";
			var requestResult = await AnkrAds.Ads.AnkrAds.DownloadAdData(testAppId, testUnitId, defaultAccount);

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