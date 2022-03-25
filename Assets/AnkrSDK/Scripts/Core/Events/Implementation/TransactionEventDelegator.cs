using System;
using AnkrSDK.Core.Events.Infrastructure;
using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Events.Implementation
{
	public class TransactionEventDelegator : ITransactionEventHandler
	{
		public event EventHandler<TransactionInput> OnTransactionSendBegin;
		public event EventHandler<TransactionInput> OnTransactionSendEnd;
		public event EventHandler<string> OnTransactionHashReceived;
		public event EventHandler<TransactionReceipt> OnReceiptReceived;
		public event EventHandler<Exception> OnError;

		public void TransactionSendBegin(TransactionInput transactionInput)
		{
			OnTransactionSendBegin?.Invoke(this, transactionInput);
		}

		public void TransactionSendEnd(TransactionInput transactionInput)
		{
			OnTransactionSendEnd?.Invoke(this, transactionInput);
		}

		public void TransactionHashReceived(string transactionHash)
		{
			OnTransactionHashReceived?.Invoke(this, transactionHash);
		}

		public void ErrorReceived(Exception exception)
		{
			OnError?.Invoke(this, exception);
		}

		public void ReceiptReceived(TransactionReceipt receipt)
		{
			OnReceiptReceived?.Invoke(this, receipt);
		}
	}
}