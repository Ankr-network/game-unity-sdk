namespace AnkrSDK.Ads.Data
{
	public enum ResponseCodeType
	{
		Success = 0,
		SessionExpired = 1,
		DeviceIdNotProvided = 1001,
		ApplicationKeyNotFound = 1002,
		DeviceNotFound = 1003,
		NoSuitableAdFound = 1004,
		IncorrectAdType = 1005,
		IncorrectPublicAddress = 1006,
		IncorrectLanguage = 1007
	}
}