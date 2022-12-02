using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Core.Utils;
using AnkrSDK.Data;
using AnkrSDK.SilentSigning.Infrastructure;
using Cysharp.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Implementation
{
	internal class Contract : IContract
	{
		private readonly string _contractABI;
		private readonly string _contractAddress;
		private readonly IEthHandler _ethHandler;
		private readonly ISilentSigningHandler _silentSigningHandler;
		private readonly IContractFunctions _contractFunctions;

		internal Contract(IEthHandler ethHandler,
			IContractFunctions contractFunctions,
			string contractAddress,
			string contractABI,
			ISilentSigningHandler silentSigningHandler
		)
		{
			_ethHandler = ethHandler;
			_contractFunctions = contractFunctions;
			_contractABI = contractABI;
			_silentSigningHandler = silentSigningHandler;
			_contractAddress = contractAddress;
		}

		public Task<TReturnType> GetData<TFieldData, TReturnType>(TFieldData requestData = null)
			where TFieldData : FunctionMessage, new()
		{
			return _contractFunctions.GetContractData<TFieldData, TReturnType>(_contractAddress, requestData);
		}

		public Task<List<EventLog<TEvDto>>> GetEvents<TEvDto>(EventFilterData evFilter)
			where TEvDto : IEventDTO, new()
		{
			var filters = EventFilterHelper.CreateEventFilters<TEvDto>(_contractAddress, evFilter);
			return _contractFunctions.GetEvents<TEvDto>(filters, _contractAddress);
		}

		public Task<List<EventLog<TEvDto>>> GetEvents<TEvDto>(EventFilterRequest<TEvDto> evFilter)
			where TEvDto : IEventDTO, new()
		{
			var filters = EventFilterHelper.CreateEventFilters(_contractAddress, evFilter);
			return _contractFunctions.GetEvents<TEvDto>(filters, _contractAddress);
		}

		public async Task<string> CallMethod(string methodName, object[] arguments = null, string gas = null,
			string gasPrice = null, string nonce = null)
		{
			var defaultAccount = await _ethHandler.GetDefaultAccount();
			var transactionInput = CreateTransactionInput(methodName, arguments, defaultAccount);
			var sendTransaction = await GetSendTransactionTask(
				defaultAccount,
				transactionInput.Data,
				gas,
				gasPrice,
				nonce
			);

			return sendTransaction;
		}

		public async Task Web3SendMethod(string methodName, object[] arguments,
			ITransactionEventHandler evController = null, string gas = null, string gasPrice = null,
			string nonce = null)
		{
			var defaultAccount = await _ethHandler.GetDefaultAccount();
			var transactionInput = CreateTransactionInput(methodName, arguments, defaultAccount);
			evController?.TransactionSendBegin(transactionInput);

			var sendTransactionTask =
				GetSendTransactionTask(defaultAccount, transactionInput.Data, gas, gasPrice, nonce);

			evController?.TransactionSendEnd(transactionInput);

			try
			{
				var response = await sendTransactionTask;

				if (!sendTransactionTask.IsFaulted)
				{
					evController?.TransactionHashReceived(response);
					await LoadReceipt(response, evController);
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

		private Task<string> GetSendTransactionTask(
			string defaultAccount,
			string transactionInputData,
			string gas,
			string gasPrice,
			string nonce)
		{
			if (_silentSigningHandler != null && _silentSigningHandler.IsSilentSigningActive())
			{
				return _silentSigningHandler.SendSilentTransaction(
					defaultAccount,
					_contractAddress,
					transactionInputData,
					gas: gas,
					gasPrice: gasPrice,
					nonce: nonce);
			}

			return _ethHandler.SendTransaction(
				defaultAccount,
				_contractAddress,
				transactionInputData,
				gas: gas,
				gasPrice: gasPrice,
				nonce: nonce
			);
		}

		public async UniTask<HexBigInteger> EstimateGas(string methodName, object[] arguments = null, string gas = null,
			string gasPrice = null, string nonce = null)
		{
			var defaultAccount = await _ethHandler.GetDefaultAccount();
			var transactionInput = CreateTransactionInput(methodName, arguments, defaultAccount);
			transactionInput.Gas = gas != null ? new HexBigInteger(gas) : null;
			transactionInput.GasPrice = gasPrice != null ? new HexBigInteger(gasPrice) : null;
			transactionInput.Nonce = nonce != null ? new HexBigInteger(nonce) : null;

			return await _ethHandler.EstimateGas(transactionInput);
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

		private TransactionInput CreateTransactionInput(string methodName, object[] arguments, string defaultAccount)
		{
			var contractBuilder = new ContractBuilder(_contractABI, _contractAddress);
			var callFunction = contractBuilder.GetFunctionBuilder(methodName);

			return callFunction.CreateTransactionInput(defaultAccount, arguments);
		}
	}
}