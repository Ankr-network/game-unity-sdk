using System;
using AnkrSDK.Core.Infrastructure;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.RPC.Eth.DTOs;
using UnityEngine;

namespace AnkrSDK.Core.Implementation
{	
	public class ContractEventSubscription<TEventDtoBase> : IContractEventSubscription where TEventDtoBase : IEventDTO, new()
	{
		public string SubscriptionId { get; set; }
		private Action<TEventDtoBase> _handler { get; }

		public ContractEventSubscription(string subscriptionId, Action<TEventDtoBase> handler)
		{
			SubscriptionId = subscriptionId;
			_handler = handler;
		}

		public void HandleMessage(RpcStreamingResponseMessage message)
		{
			try
			{
				var log = message.GetStreamingResult<FilterLog>();
				var dto = log.DecodeEvent<TEventDtoBase>();
				_handler?.Invoke(dto.Event);
			}
			catch (Exception ex)
			{
				Debug.Log("Found incompatible event format");
			}
		}
	}
}