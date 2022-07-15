using System.Runtime.InteropServices;
using AnkrAds.Ads.Infrastructure;

namespace AnkrSDK.Ads.IOS
{
	public class NativeAdsIOS : INativeAds
	{
		[DllImport("__Internal")]
		public static extern void Initialize(string appId, string deviceId, string publicAddress, string language);

		[DllImport("__Internal")]
		public static extern void Load(string unitId);

		[DllImport("__Internal")]
		public static extern void Show(string unitId);

		[DllImport("__Internal")]
		public static extern void SetCallback(CallbackDelegate callback);

		private static IOSCallbackListener _iosCallbackListener;

		public delegate void CallbackDelegate(string message);

		[AOT.MonoPInvokeCallback(typeof(CallbackDelegate))]
		private static void CallBack(string message)
		{
			_iosCallbackListener.ProcessEventMessage(message);
		}

		public void InitializeService(string appId, string deviceId, string walletAddress, string appLanguage)
		{
			SetCallback(CallBack);
			Initialize(appId, deviceId, walletAddress, appLanguage);
		}

		public IAdsEventsListener GetEventsListener()
		{
			_iosCallbackListener = new IOSCallbackListener();
			return _iosCallbackListener;
		}

		public void LoadAd(string unitID)
		{
			Load(unitID);
		}

		public void LoadAdTexture(string unitID)
		{
			Load(unitID);
		}

		public void ShowAd(string unitID)
		{
			Show(unitID);
		}
	}
}