using System;
using AnkrSDK.Core.Events.Infrastructure;
using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Events.Implementation
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