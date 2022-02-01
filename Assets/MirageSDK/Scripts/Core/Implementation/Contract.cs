using System.Collections.Generic;
using System.Threading.Tasks;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Core.Utils;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.Web3;
using WalletConnectSharp.Core.Models.Ethereum;
using WalletConnectSharp.Unity;

namespace MirageSDK.Core.Implementation
{
	internal class Contract : IContract
	{
		private readonly string _abi;
		private readonly string _address;
		private readonly IWeb3 _web3;
		private readonly IClient _client;

		public Contract(IWeb3 web3, IClient client, string address, string abi)
		{
			_web3 = web3;
			_client = client;
			_abi = abi;
			_address = address;
		}

		public Task<TReturnType> GetData<TFieldData, TReturnType>(TFieldData requestData = null)
			where TFieldData : FunctionMessage, new()
		{
			var contract = _web3.Eth.GetContractHandler(_address);
			return contract.QueryAsync<TFieldData, TReturnType>(requestData);
		}

		public Task<List<EventLog<TEvDto>>> GetAllChanges<TEvDto>() where TEvDto : IEventDTO, new()
		{
			var eventHandler = _web3.Eth.GetEvent<TEvDto>(_address);

			var filterAllTransferEventsForContract = eventHandler.CreateFilterInput();

			return eventHandler.GetAllChangesAsync(filterAllTransferEventsForContract);
		}

		public Task<string> CallMethod(string methodName, object[] arguments, string gas = null)
		{
			var activeSessionAccount = WalletConnect.ActiveSession.Accounts[0];
			var raw = _web3.Eth.GetContract(_abi, _address)
				.GetFunction(methodName)
				.CreateTransactionInput(activeSessionAccount, arguments);

			return SendTransaction(_address, raw.Data, null, gas);
		}

		public Task<Transaction> GetTransactionInfo(string receipt)
		{
			var src = new EthGetTransactionByHash(_client);
			return src.SendRequestAsync(receipt);
		}

		public async Task<string> SendTransaction(
			string to, 
			string data = null,
			string value = null,
			string gas = null)
		{
			var address = WalletConnect.ActiveSession.Accounts[0];

			var transaction = new TransactionData
			{
				from = address,
				to = to
			};

			if (data != null)
			{
				transaction.data = data;
			}

			if (value != null)
			{
				transaction.value = MirageSDKHelpers.ConvertNumber(value);
			}

			if (gas != null)
			{
				transaction.gas = MirageSDKHelpers.ConvertNumber(gas);
			}

			return await SendTransaction(transaction);
		}

		private static async Task<string> SendTransaction(TransactionData data)
		{
			return await WalletConnect.ActiveSession.EthSendTransaction(data);
		}
	}
}