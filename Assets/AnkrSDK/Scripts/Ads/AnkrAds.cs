using AnkrSDK.Ads.Data;
using AnkrSDK.Ads.Helpers;
using AnkrSDK.Core.Implementation;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.Ads
{
	public static class AnkrAds
	{
		private const string DeviceIdFieldName = "device_id";

		private const string AdServiceBaseURL = "http://45.77.189.28:5001/";
		private const string ADURL = AdServiceBaseURL + "ad";
		private const string StartURL = AdServiceBaseURL + "start";

		public static UniTask<AdData> DownloadAdData(AdType adType)
		{
			return RequestAdData(SystemInfo.deviceUniqueIdentifier, adType);
		}

		public static async UniTask<bool> Initialize(string appID)
		{
			return await RequestStartSessionData
			(appID, SystemInfo.deviceUniqueIdentifier, EthHandler.DefaultAccount,
				"EN"); //AppId : Application.identifier
		}

		private static async UniTask<AdData> RequestAdData(string deviceId, AdType adType)
		{
			var adRequestResult = await SendAdRequest(deviceId, adType);

			var code = adRequestResult.Code;
			RequestResponseCodeLogger.LogResultCode(code);

			if (code != ResponseCodeType.SessionExpired)
			{
				return string.IsNullOrEmpty(adRequestResult.Error) && code == ResponseCodeType.Success
					? adRequestResult.AdData
					: null;
			}

			const string testId = "com.ankr.test";
			await Initialize(testId);
			adRequestResult = await SendAdRequest(deviceId, adType);

			return string.IsNullOrEmpty(adRequestResult.Error)
				? adRequestResult.AdData
				: null;
		}

		private static async UniTask<bool> RequestStartSessionData(string appId, string deviceId, string publicAddress,
			string language)
		{
			var startSessionRequestResult = await SendStartSessionRequest(appId, deviceId, publicAddress, language);
			var code = startSessionRequestResult.Code;
			RequestResponseCodeLogger.LogResultCode(code);

			return code == 0;
		}

		private static UniTask<AdRequestResultResult> SendAdRequest(string deviceId, AdType adType)
		{
			const string adTypeFieldName = "ad_type";

			var form = new WWWForm();
			form.AddField(DeviceIdFieldName, deviceId);
			form.AddField(adTypeFieldName, adType.ToString());

			return AdsWebHelper.SendPostRequest<AdRequestResultResult>(ADURL, form);
		}

		private static UniTask<StartRequestResultResult> SendStartSessionRequest(string appId, string deviceId,
			string publicAddress, string language)
		{
			const string appIdFieldName = "app_id";
			const string publicAddressFieldName = "public_address";
			const string languageFieldName = "language";

			var form = new WWWForm();
			form.AddField(DeviceIdFieldName, deviceId);
			form.AddField(appIdFieldName, appId);
			form.AddField(publicAddressFieldName, publicAddress);
			form.AddField(languageFieldName, language);

			return AdsWebHelper.SendPostRequest<StartRequestResultResult>(StartURL, form);
		}

	}
}