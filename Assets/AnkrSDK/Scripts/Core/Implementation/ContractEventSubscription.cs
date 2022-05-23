using AnkrSDK.Core.Infrastructure;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.RPC.Eth.DTOs;
using System;
using UnityEngine;

namespace AnkrSDK.Core.Implementation
{
	internal class ContractEventSubscription<TEventDtoBase> : IContractEventSubscription
		where TEventDtoBase : IEventDTO, new()
	{
		public string SubscriptionId { get; }

		private readonly Action<TEventDtoBase> _handler;

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

				Debug.Log("Event Received");
				_handler?.Invoke(dto.Event);
			}
			catch (Exception)
			{
				Debug.Log("Found incompatible event format");
			}
		}
	}
}