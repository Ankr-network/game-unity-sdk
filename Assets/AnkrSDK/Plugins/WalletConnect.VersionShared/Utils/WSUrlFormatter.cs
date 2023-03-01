namespace AnkrSDK.WalletConnect.VersionShared.Utils
{
    public static class WSUrlFormatter
    {
        public static string GetReadyToUseURL(string url)
        {
            if (url.StartsWith("https"))
            {
                url = url.Replace("https", "wss");
            }
            else if (url.StartsWith("http"))
            {
                url = url.Replace("http", "ws");
            }

            return url;
        }
    }
}