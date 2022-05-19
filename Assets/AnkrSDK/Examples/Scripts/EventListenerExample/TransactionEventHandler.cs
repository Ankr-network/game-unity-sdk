using System;
using AnkrSDK.Core.Infrastructure;
using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.EventListenerExample
{
	public abstract class TransactionEventHandler : ITransactionEventHandler
	{
		public virtual void TransactionSendBegin(TransactionInput transactionInput)
		{
		}

		public virtual void TransactionSendEnd(TransactionInput transactionInput)
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