namespace AnkrSDK.Ads.Data
{
	public enum ResponseCodeType
	{
		Success = 0,
		SessionExpired = 1,
		DeviceIdNotProvided = 1001,
		DeviceNotFound = 1003,
		NoSuitableAdFound = 1004,
		IncorrectAdType = 1005,
	}
}