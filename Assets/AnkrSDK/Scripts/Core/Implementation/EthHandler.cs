using System;
using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Unity;
using Cysharp.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.Web3;

namespace AnkrSDK.Core.Implementation
{
	public class EthHandler
	{
		private readonly IWeb3 _web3Provider;

		public static string DefaultAccount
		{
			get
			{
				if (WalletConnect.ActiveSession != null)
				{
					return WalletConnect.ActiveSession.Accounts[0];
				}

				throw new Exception("Application is not linked to wallet");
			}
		}

		public EthHandler(IWeb3 web3Provider)
		{
			_web3Provider = web3Provider;
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

		public static async UniTask Disconnect(bool waitForNewSession = true)
		{
			await WalletConnect.CloseSession(waitForNewSession);
		}
	}
}