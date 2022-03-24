using System;
using AnkrSDK.Core.Events.Infrastructure;
using Nethereum.RPC.Eth.DTOs;
using UnityEngine;

namespace AnkrSDK.Examples
{
	public class LoggerEventHandler : ITransactionEventHandler
	{
		public void TransactionSendBegin(TransactionInput transactionInput)
		{
			Debug.Log("Transaction is sending");
		}

		public void TransactionSendEnd(TransactionInput transactionInput)
		{
			Debug.Log("Transaction sent");
		}

		public void TransactionHashReceived(string transactionHash)
		{
			Debug.Log($"TsransactionHash: {transactionHash}");
		}

		public void ErrorReceived(Exception exception)
		{
			Debug.LogError("Error: " + exception.Message);
		}

		public void ReceiptReceived(TransactionReceipt receipt)
		{
			Debug.Log("Receipt: " + receipt.Status);
		}
	}
}