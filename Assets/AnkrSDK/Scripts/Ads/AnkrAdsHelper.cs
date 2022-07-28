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
			#if UNITY_ANDROID
				case RuntimePlatform.Android:
				{
					return new AnkrAds.Ads.Implementation.NativeAdsAndroid();
				}
			#endif
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

	#if !UNITY_WEBGL
		public static string DeviceId => SystemInfo.deviceUniqueIdentifier;
	#else
		public static string DeviceId => WebGL.WebGLInterlayer.GetUniqueID();
	#endif

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