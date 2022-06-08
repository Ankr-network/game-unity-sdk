using System.Threading.Tasks;
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
		[SerializeField] private Button _initializeButton;
		[SerializeField] private Button _loadButton;
		[SerializeField] private Button _viewButton;
		[SerializeField] private AnkrBannerAdImage _ankrBannerAdImage;
		[SerializeField] private AnkrBannerAdSprite _ankrBannerAdSprite;
		[SerializeField] private TMP_Text _logs;

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
			
			_initializeButton.interactable = true;
			_viewButton.interactable = false;
			_loadButton.interactable = false;
			
			_initializeButton.onClick.AddListener(OnInitializeButtonClick);
			_loadButton.onClick.AddListener(OnLoadButtonClick);
			_viewButton.onClick.AddListener(OnViewButtonClick);
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
				default: Debug.LogError("WrongButtonNb");
					break;
			}
		}

		private void OnDisable()
		{
			_initializeButton.onClick.RemoveAllListeners();
			_viewButton.onClick.RemoveAllListeners();
			_loadButton.onClick.RemoveAllListeners();
			UnsubscribeToCallbackListenerEvents();
		}

		private void SubscribeToCallbackListenerEvents()
		{
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdClicked += CallbackListenerOnAdClicked;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdClosed += CallbackListenerOnAdClosed;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdFinished += CallbackListenerOnAdFinished;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdLoaded += CallbackListenerOnAdLoaded;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdOpened += CallbackListenerOnAdOpened;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdFailedToLoad += CallbackListenerOnAdFailedToLoad;
		}

		private void UpdateUILogs(string log)
		{
			_logs.text += "\n" + log;
			Debug.Log(log);
		}

		private void CallbackListenerOnAdClicked()
		{
			UpdateUILogs("CallbackListenerOnAdClicked");
			Debug.LogWarning("CallbackListenerOnAdClicked");
		}
		
		private void CallbackListenerOnAdClosed()
		{
			UpdateUILogs("CallbackListenerOnAdClosed");
			Debug.LogWarning("CallbackListenerOnAdClosed");
		}
		
		private void CallbackListenerOnAdFinished()
		{
			UpdateUILogs("CallbackListenerOnAdFinished");
			Debug.LogWarning("CallbackListenerOnAdFinished");
		}
		
		private void CallbackListenerOnAdLoaded()
		{
			UpdateUILogs("CallbackListenerOnAdLoaded");
			Debug.LogWarning("CallbackListenerOnAdLoaded");
		}
		
		private void CallbackListenerOnAdOpened()
		{
			UpdateUILogs("CallbackListenerOnAdOpened");
			Debug.LogWarning("CallbackListenerOnAdOpened");
		}
		
		private void CallbackListenerOnAdFailedToLoad()
		{
			UpdateUILogs("CallbackListenerOnAdFailedToLoad");
			Debug.LogWarning("CallbackListenerOnAdFailedToLoad");
		}

		private void UnsubscribeToCallbackListenerEvents()
		{
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdClicked -= CallbackListenerOnAdLoaded;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdClosed -= CallbackListenerOnAdLoaded;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdFinished -= CallbackListenerOnAdLoaded;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdLoaded -= CallbackListenerOnAdLoaded;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdOpened -= CallbackListenerOnAdLoaded;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdFailedToLoad -= CallbackListenerOnAdLoaded;
		}

		private void OnInitializeButtonClick()
		{
			const string walletAddress = "This is ankr mobile address";
			const string appId = "1c562170-9ee5-4157-a5f8-e99c32e73cb0";
			AnkrAds.Ads.AnkrAdsNativeAndroid.Initialize(appId, walletAddress);
			UnsubscribeToCallbackListenerEvents();
			SubscribeToCallbackListenerEvents();
			ActivateNextButton(1);
		}

		private void OnLoadButtonClick()
		{
			const string unitId = "d396af2c-aa3a-44da-ba17-68dbb7a8daa1";
			AnkrAds.Ads.AnkrAdsNativeAndroid.LoadAd(unitId);
			ActivateNextButton(2);
		}
		private void OnViewButtonClick()
		{
			const string unitId = "d396af2c-aa3a-44da-ba17-68dbb7a8daa1";
			AnkrAds.Ads.AnkrAdsNativeAndroid.ShowAd(unitId);
			ActivateNextButton(3);
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