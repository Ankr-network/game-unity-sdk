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
	public class EventFilterData
	{
		public object[] filterTopic1;
		public object[] filterTopic2;
		public object[] filterTopic3;
		public BlockParameter fromBlock;
		public BlockParameter toBlock;
	}

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

		public Task<List<EventLog<TEvDto>>> GetAllChanges<TEvDto>(EventFilterData evFilter)
			where TEvDto : IEventDTO, new()
		{
			var eventHandler = _web3.Eth.GetEvent<TEvDto>(_address);

			var filters = ApplyFilters(eventHandler, evFilter);

			return eventHandler.GetAllChangesAsync(filters);
		}

		private NewFilterInput ApplyFilters<TEvDto>(Event<TEvDto> eventHandler, EventFilterData evFilter = null)
			where TEvDto : IEventDTO, new()
		{
			NewFilterInput filters = null;
			if (evFilter == null)
			{
				filters = eventHandler.CreateFilterInput();
			}
			else
			{
				if (evFilter.filterTopic1 != null && evFilter.filterTopic2 != null && evFilter.filterTopic3 != null)
				{
					filters = eventHandler.CreateFilterInput(evFilter.filterTopic1, evFilter.filterTopic2,
						evFilter.filterTopic3, evFilter.fromBlock, evFilter.toBlock);
				}
				else if (evFilter.filterTopic1 != null && evFilter.filterTopic2 != null)
				{
					filters = eventHandler.CreateFilterInput(evFilter.filterTopic1, evFilter.filterTopic2,
						evFilter.fromBlock, evFilter.toBlock);
				}
				else if (evFilter.filterTopic1 != null)
				{
					filters = eventHandler.CreateFilterInput(evFilter.filterTopic1, evFilter.fromBlock,
						evFilter.toBlock);
				}
				else
				{
					filters = eventHandler.CreateFilterInput(evFilter.fromBlock, evFilter.toBlock);
				}
			}

			return filters;
		}

		public Task<string> CallMethod(string methodName, object[] arguments = null, string gas = null)
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