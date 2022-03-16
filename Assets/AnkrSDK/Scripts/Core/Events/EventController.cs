using System;
using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Events
{
	public class EventController
	{
		public event EventHandler<TransactionInput> OnSending;
		public event EventHandler<TransactionInput> OnSent;
		public event EventHandler<string> OnTransactionHash;
		public event EventHandler<TransactionReceipt> OnReceipt;
		public event EventHandler<Exception> OnError;

		public void InvokeSendingEvent(TransactionInput transaction)
		{
			OnSending?.Invoke(this, transaction);
		}
	
		public void InvokeSentEvent(TransactionInput transaction)
		{
			OnSent?.Invoke(this, transaction);
		}
	
		public void InvokeTransactionHashReceived(string transactionHash)
		{
			OnTransactionHash?.Invoke(this, transactionHash);
		}

		public void InvokeReceiptReceived(TransactionReceipt receipt)
		{
			OnReceipt?.Invoke(this, receipt);
		}

		public void InvokeErrorReceived(Exception error)
		{
			OnError?.Invoke(this, error);
		}
	}
}