using System;

namespace MirageSDK.WalletConnectSharp.Unity.Network.Client.Exceptions
{
	public class WebSocketInvalidArgumentException : WebSocketException
	{
		public WebSocketInvalidArgumentException()
		{
		}

		public WebSocketInvalidArgumentException(string message) : base(message)
		{
		}

		public WebSocketInvalidArgumentException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}