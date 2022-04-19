using System;

namespace AnkrSDK.WalletConnectSharp.Unity.Network.Client
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