using MirageSDK.WalletConnectSharp.Core.Models;

namespace MirageSDK.WalletConnectSharp.Unity.Network
{
	public static class NetworkMessageHelper
	{
		public static NetworkMessage GenerateAckMessage(string topic)
		{
			return new NetworkMessage
			{
				Payload = "",
				Type = "ack",
				Silent = true,
				Topic = topic
			};
		}

		public static NetworkMessage GenerateSubscribeMessage(string topic)
		{
			return new NetworkMessage
			{
				Payload = "",
				Type = "sub",
				Silent = true,
				Topic = topic
			};
		}
	}
}