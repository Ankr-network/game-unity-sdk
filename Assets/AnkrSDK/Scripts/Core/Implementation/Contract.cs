using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnkrSDK.Core.Data;
using AnkrSDK.Core.Events.Infrastructure;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Core.Utils;
using Cysharp.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace AnkrSDK.Core.Implementation
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

		public async Task<string> CallMethod(string methodName, object[] arguments = null, string gas = null,
			string gasPrice = null, string nonce = null)
		{
			var transactionInput = CreateTransactionInput(methodName, arguments);
			var sendTransaction = await AnkrWalletHelper.SendTransaction(
				EthHandler.DefaultAccount,
				_contractAddress,
				transactionInput.Data,
				gas: gas,
				gasPrice: gasPrice,
				nonce: nonce
			);

			return sendTransaction.Result;
		}

		public async Task Web3SendMethod(string methodName, object[] arguments,
			ITransactionEventHandler evController = null, string gas = null, string gasPrice = null, string nonce = null)
		{
			var transactionInput = CreateTransactionInput(methodName, arguments);

			evController?.TransactionSendBegin(transactionInput);

			var sendTransactionTask = AnkrWalletHelper.SendTransaction(
				EthHandler.DefaultAccount,
				_contractAddress,
				transactionInput.Data,
				gas: gas,
				gasPrice: gasPrice,
				nonce: nonce
			);

			evController?.TransactionSendEnd(transactionInput);

			try
			{
				var response = await sendTransactionTask;

				if (!sendTransactionTask.IsFaulted)
				{
					var transactionHash = response.Result;
					evController?.TransactionHashReceived(transactionHash);
					await LoadReceipt(transactionHash, evController);
				}
				else
				{
					evController?.ErrorReceived(sendTransactionTask.Exception);
				}
			}
			catch (Exception exception)
			{
				evController?.ErrorReceived(exception);
			}
		}

		public Task<HexBigInteger> EstimateGas(string methodName, object[] arguments = null, string gas = null,
			string gasPrice = null, string nonce = null)
		{
			var transactionInput = CreateTransactionInput(methodName, arguments);

			transactionInput.Gas = gas != null ? new HexBigInteger(gas) : null;
			transactionInput.GasPrice = gasPrice != null ? new HexBigInteger(gasPrice) : null;
			transactionInput.Nonce = nonce != null ? new HexBigInteger(nonce) : null;

			return _web3Provider.TransactionManager.EstimateGasAsync(transactionInput);
		}

		private async UniTask LoadReceipt(string transactionHash, ITransactionEventHandler evController)
		{
			var task = _ethHandler.GetTransactionReceipt(transactionHash);

			await task;

			if (!task.IsFaulted)
			{
				var receipt = task.Result;
				evController?.ReceiptReceived(receipt);
			}
			else
			{
				evController?.ErrorReceived(task.Exception);
			}
		}

		private TransactionInput CreateTransactionInput(string methodName, object[] arguments)
		{
			var activeSessionAccount = EthHandler.DefaultAccount;
			var contract = _web3Provider.Eth.GetContract(_contractABI, _contractAddress);
			var callFunction = contract.GetFunction(methodName);
			return callFunction.CreateTransactionInput(activeSessionAccount, arguments);
		}
	}
}