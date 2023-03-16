using System;
using System.Numerics;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Utils;
using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Core.Events.Model.Ethereum;
using Cysharp.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.Web3;
using UnityEngine;
using EthSendTransaction = AnkrSDK.WalletConnectSharp.Core.Events.Model.Ethereum.EthSendTransaction;

namespace AnkrSDK.Mobile
{
	public class EthHandler : IEthHandler
	{
		private readonly IWeb3 _web3Provider;
		private readonly WalletConnectSharp.Unity.WalletConnect _walletConnect;
		private readonly ISilentSigningHandler _silentSigningHandler;

		public EthHandler(IWeb3 web3Provider, ISilentSigningHandler silentSigningHandler)
		{
			_web3Provider = web3Provider;
			_silentSigningHandler = silentSigningHandler;
			_walletConnect = ConnectProvider<WalletConnectSharp.Unity.WalletConnect>.GetConnect();
		}

		public UniTask<string> WalletAddEthChain(EthChainData chainData)
		{
			if (_walletConnect.Status == WalletConnectStatus.Uninitialized)
			{
				throw new Exception("Application is not linked to wallet");
			}

			return _walletConnect.WalletAddEthChain(chainData);
		}

		public UniTask<string> WalletSwitchEthChain(EthChain chain)
		{
			if (_walletConnect.Status == WalletConnectStatus.Uninitialized)
			{
				throw new Exception("Application is not linked to wallet");
			}

			return _walletConnect.WalletSwitchEthChain(chain);
		}

		public UniTask<string> WalletUpdateEthChain(EthUpdateChainData chain)
		{
			if (_walletConnect.Status == WalletConnectStatus.Uninitialized)
			{
				throw new Exception("Application is not linked to wallet");
			}

			return _walletConnect.WalletUpdateEthChain(chain);
		}

		public UniTask<BigInteger> EthChainId()
		{
			if (_walletConnect.Status == WalletConnectStatus.Uninitialized)
			{
				throw new Exception("Application is not linked to wallet");
			}

			return _walletConnect.EthChainId();
		}

		public UniTask<string> GetDefaultAccount()
		{
			if (_walletConnect.Status == WalletConnectStatus.Uninitialized)
			{
				throw new Exception("Application is not linked to wallet");
			}

			return UniTask.FromResult(_walletConnect.GetDefaultAccount());
		}

		public UniTask<BigInteger> GetChainId()
		{
			if (_walletConnect.Status == WalletConnectStatus.Uninitialized)
			{
				throw new Exception("Application is not linked to wallet");
			}

			var chainId = _walletConnect.ChainId;
			return UniTask.FromResult(new BigInteger(chainId));
		}

		public UniTask<TransactionReceipt> GetTransactionReceipt(string transactionHash)
		{
			return _web3Provider.TransactionManager.TransactionReceiptService.PollForReceiptAsync(transactionHash)
				.AsUniTask();
		}

		public UniTask<Transaction> GetTransaction(string transactionReceipt)
		{
			var transactionByHash = new EthGetTransactionByHash(_web3Provider.Client);
			return transactionByHash.SendRequestAsync(transactionReceipt).AsUniTask();
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
			var transactionInput = new TransactionInput(to, from)
			{
				Gas = gas != null ? new HexBigInteger(gas) : null,
				GasPrice = gasPrice != null ? new HexBigInteger(gasPrice) : null,
				Nonce = nonce != null ? new HexBigInteger(nonce) : null,
				Value = value != null ? new HexBigInteger(value) : null,
				Data = data
			};

			return EstimateGas(transactionInput);
		}

		public UniTask<string> Sign(string messageToSign, string address)
		{
			if (_silentSigningHandler != null && _silentSigningHandler.IsSilentSigningActive())
			{
				_silentSigningHandler.SilentSignMessage(messageToSign, address);
			}

			return _walletConnect.EthSign(address, messageToSign);
		}

		public async UniTask<string> SendTransaction(string from, string to, string data = null, string value = null,
			string gas = null,
			string gasPrice = null, string nonce = null)
		{
			if (_silentSigningHandler != null && _silentSigningHandler.IsSilentSigningActive())
			{
				var hash = await _silentSigningHandler.SendSilentTransaction(from, to, data, value, gas, gasPrice,
					nonce);
				return hash;
			}

			var transactionData = new TransactionData
			{
				from = from, to = to, data = data,
				value = value != null ? AnkrSDKHelper.StringToBigInteger(value) : null,
				gas = gas != null ? AnkrSDKHelper.StringToBigInteger(gas) : null,
				gasPrice = gasPrice != null ? AnkrSDKHelper.StringToBigInteger(gasPrice) : null, nonce = nonce
			};
			var request = new EthSendTransaction(transactionData);
			var response = await _walletConnect
				.Send<EthSendTransaction, EthResponse>(request);
			return response.Result;
		}

		public UniTask<HexBigInteger> EstimateGas(TransactionInput transactionInput)
		{
			return _web3Provider.TransactionManager.EstimateGasAsync(transactionInput).AsUniTask();
		}

		public async UniTask<BigInteger> GetBalance(string address)
		{
			if (address == null)
			{
				address = await GetDefaultAccount();
			}

			var balance = await _web3Provider.Eth.GetBalance.SendRequestAsync(address);
			return balance.Value;
		}

		public async UniTask<BigInteger> GetBlockNumber()
		{
			var blockNumber = await _web3Provider.Eth.Blocks.GetBlockNumber.SendRequestAsync();
			return blockNumber.Value;
		}

		public async UniTask<BigInteger> GetTransactionCount(string hash)
		{
			var blockNumber = await _web3Provider.Eth.Blocks.GetBlockTransactionCountByHash.SendRequestAsync(hash);
			return blockNumber.Value;
		}

		public async UniTask<BigInteger> GetTransactionCount(BlockParameter block)
		{
			var blockNumber = await _web3Provider.Eth.Blocks.GetBlockTransactionCountByNumber.SendRequestAsync(block);
			return blockNumber.Value;
		}

		public UniTask<BlockWithTransactions> GetBlockWithTransactions(string hash)
		{
			return _web3Provider.Eth.Blocks.GetBlockWithTransactionsByHash.SendRequestAsync(hash).AsUniTask();
		}

		public UniTask<BlockWithTransactions> GetBlockWithTransactions(BlockParameter block)
		{
			return _web3Provider.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(block).AsUniTask();
		}

		public UniTask<BlockWithTransactionHashes> GetBlockWithTransactionsHashes(string hash)
		{
			return _web3Provider.Eth.Blocks.GetBlockWithTransactionsHashesByHash.SendRequestAsync(hash).AsUniTask();
		}

		public UniTask<BlockWithTransactionHashes> GetBlockWithTransactionsHashes(BlockParameter block)
		{
			return _web3Provider.Eth.Blocks.GetBlockWithTransactionsHashesByNumber.SendRequestAsync(block).AsUniTask();
		}
	}
}