using Nethereum.JsonRpc.Client.RpcMessages;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IContractEventSubscription
	{
		string SubscriptionId { get; set; }
		void HandleMessage(RpcStreamingResponseMessage message);
	}
}