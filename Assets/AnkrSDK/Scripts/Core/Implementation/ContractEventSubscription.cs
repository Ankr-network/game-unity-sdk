using System;
using AnkrSDK.Core.Data;
using AnkrSDK.Core.Infrastructure;
using Nethereum.JsonRpc.Client.RpcMessages;

namespace AnkrSDK.Core.Implementation
{	
	public class ContractEventSubscription<TEventDTOBase> : IContractEventSubscription<TEventDTOBase> where TEventDTOBase : EventDTOBase
	{
		private Action<TEventDTOBase> _handler;

		public ContractEventSubscription(Action<TEventDTOBase> handler)
		{
			_handler = handler;
		}

		public void MessageReceived(RpcStreamingResponseMessage message)
		{
			_handler?.Invoke(message.GetStreamingResult<TEventDTOBase>());
		}
	}
}