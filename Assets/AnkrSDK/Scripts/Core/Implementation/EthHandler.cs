using System;
using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Unity;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.Web3;

namespace AnkrSDK.Core.Implementation
{
	public class EthHandler
	{
		private readonly IWeb3 _web3Provider;

		public string DefaultAccount
		{
			get
			{
				if (WalletConnect.ActiveSession != null)
				{
					return WalletConnect.ActiveSession.Accounts[0];
				}
				else
				{
					throw new Exception("Application is not linked to wallet");
				}
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
			var src = new EthGetTransactionByHash(_web3Provider.Client);
			return src.SendRequestAsync(transactionReceipt);
		}
	}
}