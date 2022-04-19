using System;

namespace AnkrSDK.WalletConnectSharp.Unity.Network.Client
{
	public class WebSocketUnexpectedException : WebSocketException
	{
		public WebSocketUnexpectedException()
		{
		}

		public WebSocketUnexpectedException(string message) : base(message)
		{
		}

		public WebSocketUnexpectedException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}