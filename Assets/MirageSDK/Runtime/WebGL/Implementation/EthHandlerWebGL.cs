using System;
using System.Numerics;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Utils;
using MirageSDK.WalletConnect.VersionShared.Models.Ethereum;
using MirageSDK.WebGL.DTO;
using MirageSDK.WebGL.Extensions;
using Cysharp.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace MirageSDK.WebGL.Implementation
{
	public class EthHandlerWebGL : IEthHandler
	{
		private const bool ReturnTransactionObjects = true;
		private readonly WebGLConnect _webGLConnect;

		public EthHandlerWebGL(WebGLConnect webGLConnect)
		{
			_webGLConnect = webGLConnect;
		}

		public UniTask<string> GetDefaultAccount()
		{
			return _webGLConnect.GetDefaultAccount();
		}

		public UniTask<TransactionReceipt> GetTransactionReceipt(string transactionHash)
		{
			return _webGLConnect.GetTransactionReceipt(transactionHash);
		}

		public UniTask<Transaction> GetTransaction(string transactionHash)
		{
			return _webGLConnect.GetTransaction(transactionHash);
		}

		public UniTask<HexBigInteger> EstimateGas(TransactionInput transactionInput)
		{
			return _webGLConnect.EstimateGas(transactionInput.ToTransactionData());
		}

		public UniTask<HexBigInteger> EstimateGas(
			string from,
			string to,
			string data = null,
			string value = null,
			string gas = null,
			string gasPrice = null,
			string nonce = null
		)
		{
			var transactionData = new TransactionData
			{
				from = from, to = to, data = data,
				value = value != null ? MirageSDKHelper.StringToBigInteger(value) : null,
				gas = gas != null ? MirageSDKHelper.StringToBigInteger(gas) : null,
				gasPrice = gasPrice != null ? MirageSDKHelper.StringToBigInteger(gasPrice) : null, nonce = nonce
			};

			return _webGLConnect.EstimateGas(transactionData);
		}

		public UniTask<string> Sign(string messageToSign, string address)
		{
			var props = new DataSignaturePropsDTO
			{
				address = address, message = messageToSign
			};

			return _webGLConnect.Sign(props);
		}

		public UniTask<string> SendTransaction(string from, string to, string data = null, string value = null,
			string gas = null,
			string gasPrice = null, string nonce = null)
		{
			var transactionData = new TransactionData
			{
				from = from, to = to, data = data,
				value = value != null ? MirageSDKHelper.StringToBigInteger(value) : null,
				gas = gas != null ? MirageSDKHelper.StringToBigInteger(gas) : null,
				gasPrice = gasPrice != null ? MirageSDKHelper.StringToBigInteger(gasPrice) : null, nonce = nonce
			};

			return _webGLConnect.SendTransaction(transactionData);
		}

		public UniTask<BigInteger> GetBalance(string address = null)
		{
			return _webGLConnect.GetBalance();
		}

		public UniTask<BigInteger> GetChainId()
		{
			return _webGLConnect.GetChainId();
		}

		public UniTask WalletSwitchEthChain(EthChain chain)
		{
			return _webGLConnect.SwitchChain(chain);
		}

		public UniTask WalletAddEthChain(EthChainData chain)
		{
			return _webGLConnect.AddChain(chain);
		}

		public UniTask WalletUpdateEthChain(EthUpdateChainData chain)
		{
			return _webGLConnect.UpdateChain(chain);
		}

		public UniTask<BigInteger> EthChainId()
		{
			return _webGLConnect.GetChainId();
		}

		public UniTask<BigInteger> GetBlockNumber()
		{
			return _webGLConnect.GetBlockNumber();
		}

		public UniTask<BigInteger> GetTransactionCount(string hash)
		{
			return GetTransactionCountCommon(hash);
		}

		public UniTask<BigInteger> GetTransactionCount(BlockParameter block)
		{
			return GetTransactionCountCommon(block.GetRPCParam());
		}

		public UniTask<BlockWithTransactions> GetBlockWithTransactions(string hash)
		{
			return GetBlock<BlockWithTransactions>(hash, ReturnTransactionObjects);
		}

		public UniTask<BlockWithTransactions> GetBlockWithTransactions(BlockParameter block)
		{
			return GetBlock<BlockWithTransactions>(block.GetRPCParam(), ReturnTransactionObjects);
		}

		public UniTask<BlockWithTransactionHashes> GetBlockWithTransactionsHashes(string hash)
		{
			return GetBlock<BlockWithTransactionHashes>(hash);
		}

		public UniTask<BlockWithTransactionHashes> GetBlockWithTransactionsHashes(BlockParameter block)
		{
			return GetBlock<BlockWithTransactionHashes>(block.GetRPCParam());
		}

		private UniTask<BigInteger> GetTransactionCountCommon(string blockNumber)
		{
			return _webGLConnect.GetBlockTransactionCount(blockNumber);
		}

		private UniTask<TResultType> GetBlock<TResultType>(string blockId, bool returnTransactionObjects = false)
		{
			return _webGLConnect.GetBlock<TResultType>(blockId, returnTransactionObjects);
		}
	}
}