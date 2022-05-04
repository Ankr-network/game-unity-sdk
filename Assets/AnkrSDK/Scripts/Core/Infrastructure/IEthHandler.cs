using System.Threading.Tasks;
using AnkrSDK.Core.Implementation;
using Cysharp.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IEthHandler
	{
		Task<string> GetDefaultAccount();
		Task<TransactionReceipt> GetTransactionReceipt(string transactionHash);
		Task<Transaction> GetTransaction(string transactionReceipt);
		Task<HexBigInteger> EstimateGas(TransactionInput transactionInput);
		Task<HexBigInteger> EstimateGas(
			string from,
			string to,
			string data = null,
			string value = null,
			string gas = null,
			string gasPrice = null,
			string nonce = null
		);
	}
}