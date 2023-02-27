using System;

namespace AnkrSDK.Plugins.WalletConnectSharp.Unity.Network.Client.Exceptions
{
	public class WebSocketException : Exception
	{
		public WebSocketException()
		{
		}

		public WebSocketException(string message) : base(message)
		{
		}

		public WebSocketException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}