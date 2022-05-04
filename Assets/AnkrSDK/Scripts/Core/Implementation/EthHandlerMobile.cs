using System;
using System.Threading.Tasks;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.WalletConnectSharp.Unity;
using Cysharp.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.Web3;

namespace AnkrSDK.Core.Implementation
{
	public class EthHandlerMobile : IEthHandler
	{
		private readonly IWeb3 _web3Provider;

		public EthHandlerMobile(IWeb3 web3Provider)
		{
			_web3Provider = web3Provider;
		}
		
		public UniTask<string> GetDefaultAccount()
		{
			if (WalletConnect.ActiveSession != null)
			{
				return UniTask.FromResult(WalletConnect.ActiveSession.Accounts[0]);
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

		public async UniTask<HexBigInteger> EstimateGas(
			string from,
			string to,
			string data = null,
			string value = null,
			string gas = null,
			string gasPrice = null,
			string nonce = null
		)
		{
			var transactionInput = new TransactionInput(to, from);
			transactionInput.Gas = gas != null ? new HexBigInteger(gas) : null;
			transactionInput.GasPrice = gasPrice != null ? new HexBigInteger(gasPrice) : null;
			transactionInput.Nonce = nonce != null ? new HexBigInteger(nonce) : null;
			transactionInput.Value = value != null ? new HexBigInteger(value) : null;
			transactionInput.Data = data;
			
			return await EstimateGas(transactionInput);
		}
		
		public Task<HexBigInteger> EstimateGas(TransactionInput transactionInput)
		{
			return _web3Provider.TransactionManager.EstimateGasAsync(transactionInput);
		}
	}
}