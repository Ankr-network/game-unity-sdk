using AnkrSDK.Ads.Data;
using UnityEngine;

namespace AnkrSDK.Ads.Helpers
{
	public static class RequestResponseCodeLogger
	{
		public static void LogResultCode(ResponseCodeType code)
		{
			switch (code)
			{
				case ResponseCodeType.Success:
					Debug.Log("0 = success. Show the ad");
					break;
				case ResponseCodeType.SessionExpired:
					Debug.Log("1 = Session expired. call /start first");
					break;
				case ResponseCodeType.DeviceIdNotProvided:
					Debug.LogError("1001 = no device id provided");
					break;
				case ResponseCodeType.DeviceNotFound:
					Debug.LogError("1003 = Device not found (this can indicate you haven't call /start yet)");
					break;
				case ResponseCodeType.NoSuitableAdFound:
					Debug.LogError("1004 = No suitable ad found");
					break;
				case ResponseCodeType.IncorrectAdType:
					Debug.LogError("1005 = Incorrect app_type");
					break;
				default:
					Debug.Log($"Code = {code} received");
					break;
			}
		}
	}
}