using AnkrSDK.Plugins.WalletConnectSharp.Unity.Models.DeepLink;

namespace AnkrSDK.Plugins.WalletConnectSharp.Unity.Utils
{
	public static class MobileWalletURLFormatHelper
	{
		public static string GetURLForMobileWalletOpen(string connectURL, MobileAppData mobileAppData)
		{
			string url;
			var encodedConnect = System.Net.WebUtility.UrlEncode(connectURL);
			var universal = mobileAppData.universal;
			if (!string.IsNullOrWhiteSpace(universal))
			{
				url = $"{universal}/wc?uri={encodedConnect}";
			}
			else
			{
				var native = mobileAppData.native;
				url = $"{native}{(native.EndsWith(":") ? "//" : "/")}wc?uri={encodedConnect}";
			}

			return url;
		}
	}
}