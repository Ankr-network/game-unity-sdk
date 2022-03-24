using System;
using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Events.Infrastructure
{
	public interface ITransactionEventHandler
	{
		void EventSendBegin(TransactionInput transactionInput);
		void EventSendEnd(TransactionInput transactionInput);
		void TransactionHashReceived(string transactionHash);
		void ErrorReceived(Exception exception);
		void ReceiptReceived(TransactionReceipt receipt);
	}
}