using AnkrAds.Ads.Infrastructure;
using UnityEngine;

namespace AnkrSDK.Ads
{
	public static class AnkrAdsHelper
	{
		public static INativeAds GetNativeAdsByPlatform(RuntimePlatform runtimePlatform)
		{
			switch (runtimePlatform)
			{
				case RuntimePlatform.Android:
				{
					return new AnkrAds.Ads.Implementation.NativeAdsAndroid();
				}
			#if UNITY_IOS
				case RuntimePlatform.IPhonePlayer:
				{
					return new IOS.NativeAdsIOS();
				}
			#endif
				default:
					return new AnkrAds.Ads.Implementation.NativeAdsStandalone();
			}
		}

		public static string DeviceId => SystemInfo.deviceUniqueIdentifier;

		public static string AppLanguage
		{
			get
			{
				switch (Application.systemLanguage)
				{
					case SystemLanguage.French:
						return "FR";
					case SystemLanguage.Spanish:
						return "ES";
					case SystemLanguage.Chinese:
						return "CN";
					case SystemLanguage.Russian:
						return "RU";
					default:
						return "EN";
				}
			}
		}
	}
}