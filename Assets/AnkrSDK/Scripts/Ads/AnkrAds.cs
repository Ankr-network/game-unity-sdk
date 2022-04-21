using System;
using System.Runtime.InteropServices.WindowsRuntime;
using AnkrSDK.Ads.Data;
using AnkrSDK.Core.Implementation;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AnkrSDK.Ads
{
	public static class AnkrAds
	{
		private const string DeviceIdFieldName = "device_id";
		private const string AdTypeFieldName = "ad_type";
		private const string AppIdFieldName = "app_id";
		private const string PublicAddressFieldName = "public_address";
		private const string LanguageFieldName = "language";

		private const string RandomImageURLBase = "http://45.77.189.28:5001/ad";

		public static async UniTask<AdData> Show(AdType adType)
		{
			return await RequestAdData(SystemInfo.deviceUniqueIdentifier, adType);
		}
		
		public static async UniTask<bool> Initialize()
		{
			return await RequestStartSessionData
				(Application.identifier, SystemInfo.deviceUniqueIdentifier, EthHandler.DefaultAccount,"EN");
		}
		
		private static async UniTask<AdData> RequestAdData(string deviceId, AdType adType)
		{
			var adRequestResult = await SendAdRequest(deviceId, adType);
			
			switch (adRequestResult.Code)
			{
				case 0:
					Debug.Log("0 = success. Show the ad");
					break;
				case 1:
					Debug.Log("1 = Session expired. call /start first");
					
					await Initialize();
					adRequestResult = await SendAdRequest(deviceId, adType);
					
					break;
				case 1001:
					Debug.LogError("1001 = no device id provided");
					break;
				case 1003:
					Debug.LogError("1003 = Device not found (this can indicate you haven't call /start yet)");
					break;
				case 1004:
					Debug.LogError("1004 = No suitable ad found");
					break;
				case 1005:
					Debug.LogError("1005 = Incorrect app_type");
					break;
			}
			
			return string.IsNullOrEmpty(adRequestResult.Error) && adRequestResult.Code == 0
				? adRequestResult.AdData
				: null;
		}
		
		private static async UniTask<bool> RequestStartSessionData(string appId, string deviceId, string publicAddress, string language)
		{
			var startSessionRequestResult = await SendStartSessionRequest(appId, deviceId, publicAddress,language);
			
			switch (startSessionRequestResult.Code)
			{
				case 0:
					Debug.Log("0 = success. Show the ad");
					return true;
				case 1001:
					Debug.LogError("1001 = no device id provided");
					break;
				case 1002:
					Debug.LogError("1002 = Application key not found");
					break;
				case 1006:
					Debug.LogError("1006 = Incorrect public_address");
					break;
				case 1007:
					Debug.LogError("1007 = Incorrect language");
					break;
			}
			
			return false;
		}

		private static async UniTask<AdRequestResult> SendAdRequest(string deviceId, AdType adType)
		{
			var form = new WWWForm();
			form.AddField(DeviceIdFieldName, deviceId);
			form.AddField(AdTypeFieldName, adType.ToString());

			using (var www = UnityWebRequest.Post(RandomImageURLBase, form))
			{
				await www.SendWebRequest();

				var json = www.downloadHandler.text;

				if (www.result == UnityWebRequest.Result.Success)
				{
					try
					{
						var result = JsonConvert.DeserializeObject<AdRequestResult>(json);
						return result;
					}
					catch (Exception e)
					{
						Debug.LogError(e.Message);
						return null;
					}
				}

				Debug.LogError(www.error);
				return null;
			}
		}
		
		private static async UniTask<StartRequestResult> SendStartSessionRequest(string appId, string deviceId, string publicAddress, string language)
		{
			var form = new WWWForm();
			form.AddField(AppIdFieldName, appId);
			form.AddField(DeviceIdFieldName, deviceId);
			form.AddField(PublicAddressFieldName, publicAddress);
			form.AddField(LanguageFieldName, language);

			using (var www = UnityWebRequest.Post(RandomImageURLBase, form))
			{
				await www.SendWebRequest();

				var json = www.downloadHandler.text;

				if (www.result == UnityWebRequest.Result.Success)
				{
					try
					{
						var result = JsonConvert.DeserializeObject<StartRequestResult>(json);
						return result;
					}
					catch (Exception e)
					{
						Debug.LogError(e.Message);
						return null;
					}
				}

				Debug.LogError(www.error);
				return null;
			}
		}
	}
}