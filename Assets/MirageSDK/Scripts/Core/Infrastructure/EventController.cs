using System;
using Nethereum.RPC.Eth.DTOs;

namespace MirageSDK.Core.Infrastructure
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
	
		public void SetTransactionHash(string transactionHash)
		{
			OnTransactionHash?.Invoke(this, transactionHash);
		}

		public void SetReceipt(TransactionReceipt receipt)
		{
			OnReceipt?.Invoke(this, receipt);
		}

		public void SetError(Exception error)
		{
			OnError?.Invoke(this, error);
		}
	}
}