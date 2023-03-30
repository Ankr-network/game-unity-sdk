using System;

namespace MirageSDK.WalletConnectSharp.Unity.Network.Client.Exceptions
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