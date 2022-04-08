using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Implementation;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Infrastructure;

namespace AnkrSDK.WalletConnectSharp.Unity.Network.Client.Helpers
{
	public static class WebSocketFactory
	{
		/// <summary>
		/// Create WebSocket client instance
		/// </summary>
		/// <returns>The WebSocket instance.</returns>
		/// <param name="url">WebSocket valid URL.</param>
		public static IWebSocket CreateInstance(string url)
		{
		#if !UNITY_WEBGL || UNITY_EDITOR
			return new WebSocket(url);
		#else
			return new WebGLWebSocket(url);
		#endif
		}
	}
}