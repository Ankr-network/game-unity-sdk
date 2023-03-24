using MirageSDK.WebGL.DTO;
using Nethereum.RPC.Eth.DTOs;

namespace MirageSDK.WebGL.Extensions
{
	public static class TransactionInputExtension
	{
		public static TransactionData ToTransactionData(this TransactionInput data)
		{
			var transactionData = new TransactionData
			{
				@from = data.From,
				to = data.To,
				data = data.Data,
				gas = data.Gas != null ? data.Gas.Value.ToString() : null,
				gasPrice = data.GasPrice != null ? data.GasPrice.Value.ToString() : null,
				nonce = data.Nonce != null ? data.Nonce.Value.ToString() : null,
				value = data.Value != null ? data.Value.HexValue : null,
			};
			return transactionData;
		}

		public static TransactionData ToTransactionData(this CallInput data)
		{
			var transactionData = new TransactionData
			{
				@from = data.From,
				to = data.To,
				data = data.Data,
				gas = data.Gas != null ? data.Gas.Value.ToString() : null,
				gasPrice = data.GasPrice != null ? data.GasPrice.Value.ToString() : null,
				value = data.Value != null ? data.Value.HexValue : null,
			};
			return transactionData;
		}
	}
}