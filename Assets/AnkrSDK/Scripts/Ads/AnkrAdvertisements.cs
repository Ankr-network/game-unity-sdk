using System;
using AnkrAds.Ads.Infrastructure;
using UnityEngine;

namespace AnkrSDK.Ads
{
	public static class AnkrAdvertisements
	{
		private static INativeAds _nativeAds;

		public static event Action AdInitialized;
		public static event Action<string> AdLoaded;
		public static event Action<string> AdFailedToLoad;
		public static event Action<string> AdClicked;
		public static event Action AdOpened;
		public static event Action AdClosed;
		public static event Action AdFinished;
		public static event Action<string> AdRewarded;
		public static event Action<string, byte[]> AdTextureReceived;
		public static event Action<string> Error;

		public static void Initialize(string appId, string accountAddress)
		{
			_nativeAds = AnkrAdsHelper.GetNativeAdsByPlatform(Application.platform);
			_nativeAds.InitializeService(
				appId,
				AnkrAdsHelper.DeviceId,
				accountAddress,
				AnkrAdsHelper.AppLanguage
			);
			SubscribeToCallbackEvents(_nativeAds.GetEventsListener());
		}

		public static void LoadAd(string unitId)
		{
			_nativeAds.LoadAd(unitId);
		}

		public static void LoadAdTexture(string unitId)
		{
			_nativeAds.LoadAdTexture(unitId);
		}

		public static void ShowAd(string unitId)
		{
			_nativeAds.ShowAd(unitId);
		}

		private static void SubscribeToCallbackEvents(IAdsEventsListener callbackListener)
		{
			callbackListener.AdInitialized += OnAdInitialized;
			callbackListener.AdLoaded += OnAdLoaded;
			callbackListener.AdFailedToLoad += OnAdFailedToLoad;
			callbackListener.AdClicked += OnAdClicked;
			callbackListener.AdOpened += OnAdOpened;
			callbackListener.AdClosed += OnAdClosed;
			callbackListener.AdFinished += OnAdFinished;
			callbackListener.AdRewarded += OnAdRewarded;
			callbackListener.AdTextureReceived += OnAdTextureReceived;
			callbackListener.Error += OnError;
		}
		
		private static void OnAdInitialized()
		{
			AdInitialized?.Invoke();
		}

		private static void OnAdLoaded(string uuid)
		{
			AdLoaded?.Invoke(uuid);
		}

		private static void OnAdFailedToLoad(string uuid)
		{
			AdFailedToLoad?.Invoke(uuid);
		}

		private static void OnAdClicked(string uuid)
		{
			AdClicked?.Invoke(uuid);
		}

		private static void OnAdOpened()
		{
			AdOpened?.Invoke();
		}

		private static void OnAdClosed()
		{
			AdClosed?.Invoke();
		}

		private static void OnAdFinished()
		{
			AdFinished?.Invoke();
		}

		private static void OnAdRewarded(string uuid)
		{
			AdRewarded?.Invoke(uuid);
		}

		private static void OnAdTextureReceived(string unitID, byte[] adTextureData)
		{
			AdTextureReceived?.Invoke(unitID, adTextureData);
		}

		private static void OnError(string errorMessage)
		{
			Error?.Invoke(errorMessage);
		}
	}
}