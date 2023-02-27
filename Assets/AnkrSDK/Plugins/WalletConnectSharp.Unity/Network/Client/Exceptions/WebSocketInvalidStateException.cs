using System;

namespace AnkrSDK.Plugins.WalletConnectSharp.Unity.Network.Client.Exceptions
{
	public class WebSocketInvalidStateException : WebSocketException
	{
		public WebSocketInvalidStateException()
		{
		}

		public WebSocketInvalidStateException(string message) : base(message)
		{
		}

		public WebSocketInvalidStateException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}