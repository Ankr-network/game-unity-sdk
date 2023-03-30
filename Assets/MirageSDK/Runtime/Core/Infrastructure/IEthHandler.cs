using System.Numerics;
using MirageSDK.WalletConnect.VersionShared.Models.Ethereum;
using Cysharp.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace MirageSDK.Core.Infrastructure
{
	public interface IEthHandler
	{
		UniTask<string> GetDefaultAccount();
		UniTask<TransactionReceipt> GetTransactionReceipt(string transactionHash);
		UniTask<Transaction> GetTransaction(string transactionHash);
		UniTask<HexBigInteger> EstimateGas(TransactionInput transactionInput);

		UniTask<HexBigInteger> EstimateGas(
			string from,
			string to,
			string data = null,
			string value = null,
			string gas = null,
			string gasPrice = null,
			string nonce = null
		);

		UniTask<string> Sign(string messageToSign, string address);

		UniTask<string> SendTransaction(
			string from,
			string to,
			string data = null,
			string value = null,
			string gas = null,
			string gasPrice = null,
			string nonce = null
		);

		UniTask<BigInteger> GetBalance(string address = null);
		UniTask<BigInteger> GetBlockNumber();
		UniTask<BigInteger> GetTransactionCount(string hash);
		UniTask<BigInteger> GetTransactionCount(BlockParameter block);
		UniTask<BlockWithTransactions> GetBlockWithTransactions(string hash);
		UniTask<BlockWithTransactions> GetBlockWithTransactions(BlockParameter block);
		UniTask<BlockWithTransactionHashes> GetBlockWithTransactionsHashes(string hash);
		UniTask<BlockWithTransactionHashes> GetBlockWithTransactionsHashes(BlockParameter block);
		UniTask<BigInteger> GetChainId();
		UniTask WalletAddEthChain(EthChainData chainData);
		UniTask WalletSwitchEthChain(EthChain chain);
		UniTask WalletUpdateEthChain(EthUpdateChainData chain);
		UniTask<BigInteger> EthChainId();
	}
}