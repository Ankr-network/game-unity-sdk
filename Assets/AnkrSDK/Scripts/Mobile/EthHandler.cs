using System;
using System.Threading.Tasks;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Utils;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using AnkrSDK.WalletConnectSharp.Unity;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.Web3;
using UnityEngine;

namespace AnkrSDK.Mobile
{
	public class EthHandler : IEthHandler
	{
		private readonly IWeb3 _web3Provider;
		private readonly WalletConnect _walletConnect;

		public EthHandler(IWeb3 web3Provider)
		{
			_web3Provider = web3Provider;
			_walletConnect = WalletConnectProvider.GetWalletConnect();
		}

		public Task<string> GetDefaultAccount()
		{
			if (_walletConnect.Session != null)
			{
				var activeSessionAccount = _walletConnect.Session.Accounts[0];
				if (string.IsNullOrEmpty(activeSessionAccount))
				{
					Debug.LogError("Account is null");
				}

				return Task.FromResult(activeSessionAccount);
			}

			throw new Exception("Application is not linked to wallet");
		}

		public Task<TransactionReceipt> GetTransactionReceipt(string transactionHash)
		{
			return _web3Provider.TransactionManager.TransactionReceiptService.PollForReceiptAsync(transactionHash);
		}

		public Task<Transaction> GetTransaction(string transactionReceipt)
		{
			var transactionByHash = new EthGetTransactionByHash(_web3Provider.Client);
			return transactionByHash.SendRequestAsync(transactionReceipt);
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

		public Task<string> Sign(string messageToSign, string address)
		{
			return _walletConnect.Session.EthSign(address, messageToSign);
		}

		public async Task<string> SendTransaction(string from, string to, string data = null, string value = null,
			string gas = null,
			string gasPrice = null, string nonce = null)
		{
			var transactionData = new TransactionData
			{
				from = from,
				to = to,
				data = data,
				value = value != null ? AnkrSDKHelper.StringToBigInteger(value) : null,
				gas = gas != null ? AnkrSDKHelper.StringToBigInteger(gas) : null,
				gasPrice = gasPrice != null ? AnkrSDKHelper.StringToBigInteger(gasPrice) : null,
				nonce = nonce
			};

			var request = new AnkrSDK.WalletConnectSharp.Core.Models.Ethereum.EthSendTransaction(transactionData);
			var response = await _walletConnect.Session
				.Send<AnkrSDK.WalletConnectSharp.Core.Models.Ethereum.EthSendTransaction, EthResponse>(request);
			return response.Result;
		}

		public Task<HexBigInteger> EstimateGas(TransactionInput transactionInput)
		{
			return _web3Provider.TransactionManager.EstimateGasAsync(transactionInput);
		}
	}
}