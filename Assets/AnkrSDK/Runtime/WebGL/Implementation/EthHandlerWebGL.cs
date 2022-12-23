using System;
using System.Numerics;
using AnkrSDK.WebGL.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Utils;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using AnkrSDK.WebGL.DTO;

namespace AnkrSDK.WebGL.Implementation
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

		public Task<string> GetDefaultAccount()
		{
			return _webGlWrapper.GetDefaultAccount();
		}

		public Task<TransactionReceipt> GetTransactionReceipt(string transactionHash)
		{
			return _webGlWrapper.GetTransactionReceipt(transactionHash);
		}

		public Task<Transaction> GetTransaction(string transactionReceipt)
		{
			return _webGlWrapper.GetTransaction(transactionReceipt);
		}

		public Task<HexBigInteger> EstimateGas(TransactionInput transactionInput)
		{
			return _webGlWrapper.EstimateGas(transactionInput.ToTransactionData());
		}

		public Task<HexBigInteger> EstimateGas(
			string from,
			string to,
			string data = null,
			string value = null,
			string gas = null,
			string gasPrice = null,
			string nonce = null
		)
		{
			var transactionData = new DTO.TransactionData
			{
				from = from,
				to = to,
				data = data,
				value = value != null ? AnkrSDKHelper.StringToBigInteger(value) : null,
				gas = gas != null ? AnkrSDKHelper.StringToBigInteger(gas) : null,
				gasPrice = gasPrice != null ? AnkrSDKHelper.StringToBigInteger(gasPrice) : null,
				nonce = nonce
			};

			return _webGlWrapper.EstimateGas(transactionData);
		}

		public Task<string> Sign(string messageToSign, string address)
		{
			var props = new DTO.DataSignaturePropsDTO
			{
				address = address,
				message = messageToSign
			};

			return _webGlWrapper.Sign(props);
		}

		public Task<string> SendTransaction(string from, string to, string data = null, string value = null,
			string gas = null,
			string gasPrice = null, string nonce = null)
		{
			var transactionData = new DTO.TransactionData
			{
				from = from,
				to = to,
				data = data,
				value = value != null ? AnkrSDKHelper.StringToBigInteger(value) : null,
				gas = gas != null ? AnkrSDKHelper.StringToBigInteger(gas) : null,
				gasPrice = gasPrice != null ? AnkrSDKHelper.StringToBigInteger(gasPrice) : null,
				nonce = nonce
			};

			return _webGlWrapper.SendTransaction(transactionData);
		}

		public async Task<BigInteger> GetBalance(string address = null)
		{
			address = await GetDefaultAccount();
			var callObject = new WebGLCallObject
			{
				Path = GetBalanceMethodName,
				Args = address != null ? new[] {address} : null
			};
			var balance = await _webGlWrapper.CallMethod<BigInteger>(callObject);
			return balance;
		}
		
		public Task<BigInteger> GetChainId()
		{
			return _webGlWrapper.GetChainId();
		}

		public Task<string> WalletSwitchEthChain(EthChainData chainData)
		{
			//TODO https://ankrnetwork.atlassian.net/browse/MC-75
		}

		public Task<string> WalletAddEthChain(EthChainData chainData)
		{
			//TODO https://ankrnetwork.atlassian.net/browse/MC-75
		}

		public Task<BigInteger> GetBlockNumber()
		{
			var callObject = new WebGLCallObject
			{
				Path = GetBlockNumberMethodName
			};
			return _webGlWrapper.CallMethod<BigInteger>(callObject);
		}

		public Task<BigInteger> GetTransactionCount(string hash)
		{
			return GetTransactionCountCommon(hash);
		}

		public Task<BigInteger> GetTransactionCount(BlockParameter block)
		{
			return GetTransactionCountCommon(block.GetRPCParam());
		}

		public Task<BlockWithTransactions> GetBlockWithTransactions(string hash)
		{
			return GetBlock<BlockWithTransactions>(hash, ReturnTransactionObjects);
		}

		public Task<BlockWithTransactions> GetBlockWithTransactions(BlockParameter block)
		{
			return GetBlock<BlockWithTransactions>(block.GetRPCParam(), ReturnTransactionObjects);
		}

		public Task<BlockWithTransactionHashes> GetBlockWithTransactionsHashes(string hash)
		{
			return GetBlock<BlockWithTransactionHashes>(hash);
		}

		public Task<BlockWithTransactionHashes> GetBlockWithTransactionsHashes(BlockParameter block)
		{
			return GetBlock<BlockWithTransactionHashes>(block.GetRPCParam());
		}

		private Task<BigInteger> GetTransactionCountCommon(string blockId)
		{
			var callObject = new WebGLCallObject
			{
				Path = GetBlockTransactionCountMethodName,
				Args = new[] {blockId}
			};
			return _webGlWrapper.CallMethod<BigInteger>(callObject);
		}

		private Task<TResultType> GetBlock<TResultType>(string blockId, bool returnTransactionObjects = false)
		{
			var callObject = new WebGLCallObject
			{
				Path = GetBlockMethodName,
				Args = new object[] {blockId, returnTransactionObjects}
			};
			return _webGlWrapper.CallMethod<TResultType>(callObject);
		}
	}
}