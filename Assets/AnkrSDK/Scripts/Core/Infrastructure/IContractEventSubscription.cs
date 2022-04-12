using Nethereum.JsonRpc.Client.RpcMessages;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IContractEventSubscription
	{
		string SubscriptionId { get; }
		void HandleMessage(RpcStreamingResponseMessage message);
	}
}