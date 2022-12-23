using System.Numerics;
using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
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

		Task<string> Sign(string messageToSign, string address);

		Task<string> SendTransaction(
			string from,
			string to,
			string data = null,
			string value = null,
			string gas = null,
			string gasPrice = null,
			string nonce = null
		);

		Task<BigInteger> GetBalance(string address = null);
		Task<BigInteger> GetBlockNumber();
		Task<BigInteger> GetTransactionCount(string hash);
		Task<BigInteger> GetTransactionCount(BlockParameter block);
		Task<BlockWithTransactions> GetBlockWithTransactions(string hash);
		Task<BlockWithTransactions> GetBlockWithTransactions(BlockParameter block);
		Task<BlockWithTransactionHashes> GetBlockWithTransactionsHashes(string hash);
		Task<BlockWithTransactionHashes> GetBlockWithTransactionsHashes(BlockParameter block);
		Task<BigInteger> GetChainId();
		Task<string> WalletAddEthChain(EthChainData chainData);
		Task<string> WalletSwitchEthChain(EthChain chain);
	}
}