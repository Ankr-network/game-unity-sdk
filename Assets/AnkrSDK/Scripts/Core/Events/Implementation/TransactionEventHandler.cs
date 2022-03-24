using System;
using AnkrSDK.Core.Events.Infrastructure;
using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Events.Implementation
{
	public abstract class TransactionEventHandler : ITransactionEventHandler
	{
		public virtual void EventSendBegin(TransactionInput transactionInput)
		{
		}

		public virtual void EventSendEnd(TransactionInput transactionInput)
		{
		}

		public virtual void TransactionHashReceived(string transactionHash)
		{
		}

		public virtual void ErrorReceived(Exception exception)
		{
		}

		public virtual void ReceiptReceived(TransactionReceipt receipt)
		{
		}
	}
}