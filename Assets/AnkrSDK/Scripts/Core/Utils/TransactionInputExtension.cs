using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Eth.DTOs;
using UnityEngine;

namespace AnkrSDK.Core.Utils
{
	public static class TransactionInputExtension
	{
		public static TransactionData ToTransactionData(this TransactionInput data)
		{
			var transactionData = new TransactionData
			{
				from = data.From,
				to = data.To,
				data = data.Data,
				gas = data.Gas != null ? data.Gas.Value.ToString() : null,
				gasPrice = data.GasPrice != null ? data.GasPrice.Value.ToString() : null,
				nonce = data.Nonce != null ? data.Nonce.Value.ToString() : null,
				value = data.Value != null ? data.Value.Value.ToString() : null,
			};
			return transactionData;
		}
	}
}