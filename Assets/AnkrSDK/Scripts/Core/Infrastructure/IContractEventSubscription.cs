using Nethereum.JsonRpc.Client.RpcMessages;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IContractEventSubscription
	{
		void HandleMessage(RpcStreamingResponseMessage message);
	}
}