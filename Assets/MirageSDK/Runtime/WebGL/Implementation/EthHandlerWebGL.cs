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
using TransactionData = MirageSDK.WebGL.DTO.TransactionData;

namespace MirageSDK.WebGL.Implementation
{
	public class EthHandlerWebGL : IEthHandler
	{
		private const string GetBalanceMethodName = "eth.getBalance";
		private const string GetBlockMethodName = "eth.getBlock";
		private const string GetBlockNumberMethodName = "eth.getBlockNumber";
		private const string GetBlockTransactionCountMethodName = "eth.getBlockTransactionCount";
		private const bool ReturnTransactionObjects = true;
		private readonly WebGLWrapper _webGlWrapper;

		public EthHandlerWebGL(WebGLWrapper webGlWrapper)
		{
			_webGlWrapper = webGlWrapper;
		}

		public UniTask<string> GetDefaultAccount()
		{
			return _webGlWrapper.GetDefaultAccount();
		}

		public UniTask<TransactionReceipt> GetTransactionReceipt(string transactionHash)
		{
			return _webGlWrapper.GetTransactionReceipt(transactionHash);
		}

		public UniTask<Transaction> GetTransaction(string transactionHash)
		{
			return _webGlWrapper.GetTransaction(transactionHash);
		}

		public UniTask<HexBigInteger> EstimateGas(TransactionInput transactionInput)
		{
			return _webGlWrapper.EstimateGas(transactionInput.ToTransactionData());
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

			return _webGlWrapper.EstimateGas(transactionData);
		}

		public UniTask<string> Sign(string messageToSign, string address)
		{
			var props = new DataSignaturePropsDTO
			{
				address = address, message = messageToSign
			};

			return _webGlWrapper.Sign(props);
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

			return _webGlWrapper.SendTransaction(transactionData);
		}

		public async UniTask<BigInteger> GetBalance(string address = null)
		{
			address = await GetDefaultAccount();
			var callObject = new WebGLCallObject
			{
				Path = GetBalanceMethodName, Args = address != null
					? new[]
					{
						address
					}
					: null
			};
			var balance = await _webGlWrapper.CallMethod<BigInteger>(callObject);
			return balance;
		}

		public UniTask<BigInteger> GetChainId()
		{
			return _webGlWrapper.GetChainId();
		}

		public UniTask<string> WalletSwitchEthChain(EthChain chain)
		{
			return _webGlWrapper.SwitchChain(chain);
		}

		public UniTask<string> WalletAddEthChain(EthChainData chain)
		{
			return _webGlWrapper.AddChain(chain);
		}

		public UniTask<string> WalletUpdateEthChain(EthUpdateChainData chain)
		{
			throw new NotImplementedException();
		}

		public UniTask<BigInteger> EthChainId()
		{
			return _webGlWrapper.GetChainId();
		}

		public UniTask<BigInteger> GetBlockNumber()
		{
			var callObject = new WebGLCallObject
			{
				Path = GetBlockNumberMethodName
			};
			return _webGlWrapper.CallMethod<BigInteger>(callObject);
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

		private UniTask<BigInteger> GetTransactionCountCommon(string blockId)
		{
			var callObject = new WebGLCallObject
			{
				Path = GetBlockTransactionCountMethodName, Args = new[]
				{
					blockId
				}
			};
			return _webGlWrapper.CallMethod<BigInteger>(callObject);
		}

		private UniTask<TResultType> GetBlock<TResultType>(string blockId, bool returnTransactionObjects = false)
		{
			var callObject = new WebGLCallObject
			{
				Path = GetBlockMethodName, Args = new object[]
				{
					blockId, returnTransactionObjects
				}
			};
			return _webGlWrapper.CallMethod<TResultType>(callObject);
		}
	}
}