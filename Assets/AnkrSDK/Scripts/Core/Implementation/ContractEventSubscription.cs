using System;
using AnkrSDK.Core.Infrastructure;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.JsonRpc.Client.RpcMessages;

namespace AnkrSDK.Core.Implementation
{	
	public class ContractEventSubscription<TEventDtoBase> : IContractEventSubscription where TEventDtoBase : IEventDTO, new()
	{
		private readonly Action<TEventDtoBase> _handler;

		public ContractEventSubscription(Action<TEventDtoBase> handler)
		{
			_handler = handler;
		}

		public void HandleMessage(RpcStreamingResponseMessage message)
		{
			_handler?.Invoke(message.GetStreamingResult<TEventDtoBase>());
		}
	}
}