using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MirageSDK.Core.Data;
using MirageSDK.Core.Events;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Core.Utils;
using MirageSDK.WalletConnectSharp.Unity;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace MirageSDK.Core.Implementation
{
	internal class Contract : IContract
	{
		private readonly string _contractABI;
		private readonly string _contractAddress;
		private readonly IWeb3 _web3Provider;

		private readonly EthHandler _ethHandler;

		internal Contract(IWeb3 web3Provider, EthHandler ethHandler, string contractAddress, string contractABI)
		{
			_web3Provider = web3Provider;
			_ethHandler = ethHandler;
			_contractABI = contractABI;
			_contractAddress = contractAddress;
		}

		public Task<TReturnType> GetData<TFieldData, TReturnType>(TFieldData requestData = null)
			where TFieldData : FunctionMessage, new()
		{
			var contract = _web3Provider.Eth.GetContractHandler(_contractAddress);
			return contract.QueryAsync<TFieldData, TReturnType>(requestData);
		}

		public Task<List<EventLog<TEvDto>>> GetAllChanges<TEvDto>(EventFilterData evFilter)
			where TEvDto : IEventDTO, new()
		{
			var eventHandler = _web3Provider.Eth.GetEvent<TEvDto>(_contractAddress);

			var filters = EventFilterHelper.CreateEventFilters(eventHandler, evFilter);

			return eventHandler.GetAllChangesAsync(filters);
		}

		public Task<string> CallMethod(string methodName, object[] arguments = null, string gas = null,
			string gasPrice = null, string nonce = null)
		{
			var transactionInput = CreateTransactionInput(methodName, arguments);
			return MirageWalletHelper.SendTransaction(_contractAddress, transactionInput.Data, gas: gas);
		}

		public async Task Web3SendMethod(string methodName, object[] arguments,
			EventController evController = null, string gas = null, string gasPrice = null, string nonce = null)
		{
			if (evController == null)
			{
				evController = new EventController();
			}

			var transactionInput = CreateTransactionInput(methodName, arguments);

			evController.InvokeSendingEvent(transactionInput);

			var sendTransactionTask =
				MirageWalletHelper.SendTransaction(_contractAddress, transactionInput.Data, gas: gas);

			evController.InvokeSentEvent(transactionInput);

			await sendTransactionTask;

			if (!sendTransactionTask.IsFaulted)
			{
				var transactionHash = sendTransactionTask.Result;
				evController.InvokeTransactionHashReceived(transactionHash);
				await LoadReceipt(transactionHash, evController);
			}
			else
			{
				evController.InvokeErrorReceived(sendTransactionTask.Exception);
			}
		}

		private async UniTask LoadReceipt(string transactionHash, EventController evController)
		{
			var task = _ethHandler.GetTransactionReceipt(transactionHash);

			await task;
			
			if (!task.IsFaulted)
			{
				var receipt = task.Result;
				evController.InvokeReceiptReceived(receipt);
			}
			else
			{
				evController.InvokeErrorReceived(task.Exception);
			}
		}

		private TransactionInput CreateTransactionInput(string methodName, object[] arguments)
		{
			var activeSessionAccount = WalletConnect.ActiveSession.Accounts[0];
			var contract = _web3Provider.Eth.GetContract(_contractABI, _contractAddress);
			var callFunction = contract.GetFunction(methodName);
			return callFunction.CreateTransactionInput(activeSessionAccount, arguments);
		}
	}
}