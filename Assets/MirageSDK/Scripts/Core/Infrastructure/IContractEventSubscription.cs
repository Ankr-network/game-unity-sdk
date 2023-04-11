using Nethereum.JsonRpc.Client.RpcMessages;

namespace MirageSDK.Core.Infrastructure
{
	public interface IContractEventSubscription
	{
		string SubscriptionId { get; }
		void HandleMessage(RpcStreamingResponseMessage message);
	}
}