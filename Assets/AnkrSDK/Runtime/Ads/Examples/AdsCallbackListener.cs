using AnkrSDK.Ads.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.Ads.Examples
{
	public class AdsCallbackListener : MonoBehaviour
	{
		[SerializeField] private Button _initializeButton;
		[SerializeField] private Button _loadFullscreenAdButton;
		[SerializeField] private Button _loadBannerAdButton;
		[SerializeField] private Button _viewButton;
		[SerializeField] private AnkrBannerAdImage _ankrBannerAdImage;
		[SerializeField] private AnkrBannerAdSprite _ankrBannerAdSprite;
		[SerializeField] private TMP_Text _logs;

		private void OnEnable()
		{
			ActivateNextButton(0);

			var isPhone = IsMobilePlatform();
			_loadFullscreenAdButton.gameObject.SetActive(isPhone);
			_viewButton.gameObject.SetActive(isPhone);
		}

		public void SubscribeToCallbackListenerEvents()
		{
			AnkrAdvertisements.AdInitialized += CallbackListenerOnAdInitialized;
			AnkrAdvertisements.AdClicked += CallbackListenerOnAdClicked;
			AnkrAdvertisements.AdClosed += CallbackListenerOnAdClosed;
			AnkrAdvertisements.AdFinished += CallbackListenerOnAdFinished;
			AnkrAdvertisements.AdLoaded += CallbackListenerOnAdLoaded;
			AnkrAdvertisements.AdOpened += CallbackListenerOnAdOpened;
			AnkrAdvertisements.AdRewarded += CallbackListenerOnAdRewarded;
			AnkrAdvertisements.AdFailedToLoad += CallbackListenerOnAdFailedToLoad;
			AnkrAdvertisements.AdTextureReceived += CallbackListenerOnAdTextureReceived;
			AnkrAdvertisements.Error += CallbackListenerOnError;
		}

		public void UnsubscribeToCallbackListenerEvents()
		{
			AnkrAdvertisements.AdInitialized -= CallbackListenerOnAdInitialized;
			AnkrAdvertisements.AdClicked -= CallbackListenerOnAdClicked;
			AnkrAdvertisements.AdClosed -= CallbackListenerOnAdClosed;
			AnkrAdvertisements.AdFinished -= CallbackListenerOnAdFinished;
			AnkrAdvertisements.AdLoaded -= CallbackListenerOnAdLoaded;
			AnkrAdvertisements.AdOpened -= CallbackListenerOnAdOpened;
			AnkrAdvertisements.AdRewarded -= CallbackListenerOnAdRewarded;
			AnkrAdvertisements.AdFailedToLoad -= CallbackListenerOnAdFailedToLoad;
			AnkrAdvertisements.AdTextureReceived -= CallbackListenerOnAdTextureReceived;
			AnkrAdvertisements.Error -= CallbackListenerOnError;
		}

		public Button GetInitializeButton()
		{
			return _initializeButton;
		}

		public Button GetLoadFullscreenAdButton()
		{
			return _loadFullscreenAdButton;
		}

		public Button GetLoadBannerAdButton()
		{
			return _loadBannerAdButton;
		}

		public Button GetViewButton()
		{
			return _viewButton;
		}

		public void ActivateBillboardAds(bool isActive)
		{
			_ankrBannerAdImage.gameObject.SetActive(isActive);
			_ankrBannerAdSprite.gameObject.SetActive(isActive);
		}

		private bool IsMobilePlatform()
		{
			return Application.platform == RuntimePlatform.Android ||
			       Application.platform == RuntimePlatform.IPhonePlayer;
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

			if (uuid == AdsBackendInformation.FullscreenAdTestUnitId)
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