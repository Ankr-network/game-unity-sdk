using AnkrSDK.Plugins.WalletConnectSharp.Unity.Network.Client.Implementation;
using AnkrSDK.Plugins.WalletConnectSharp.Unity.Network.Client.Infrastructure;

namespace AnkrSDK.Plugins.WalletConnectSharp.Unity.Network.Client
{
	public static class WebSocketFactory
	{
		public static IWebSocket CreateInstance(string url)
		{
		#if UNITY_WEBGL && !UNITY_EDITOR
			return new WebGLWebSocket(url);
		#else
			return new WebSocket(url);
		#endif
		}
	}
}