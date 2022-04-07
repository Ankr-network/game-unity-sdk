using System;
using AnkrSDK.Core.Data;
using AnkrSDK.Core.Implementation;
using Nethereum.JsonRpc.Client.RpcMessages;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IContractEventSubscription<in TEventDTOBase> where TEventDTOBase : EventDTOBase
	{
		void MessageReceived(RpcStreamingResponseMessage message);
	}
}