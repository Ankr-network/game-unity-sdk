using System.Collections.Generic;
using AnkrSDK.Data;
using Cysharp.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IContract
	{
		UniTask<string> CallMethod(string methodName, object[] arguments = null, string gas = null, string gasPrice = null,
			string nonce = null);

		UniTask Web3SendMethod(string methodName, object[] arguments, ITransactionEventHandler evController = null,
			string gas = null, string gasPrice = null, string nonce = null);

		UniTask<List<EventLog<TEvDto>>> GetEvents<TEvDto>(EventFilterData evFilter = null)
			where TEvDto : IEventDTO, new();

		UniTask<List<EventLog<TEvDto>>> GetEvents<TEvDto>(EventFilterRequest<TEvDto> evFilter = null)
			where TEvDto : IEventDTO, new();

		UniTask<TReturnType> GetData<TFieldData, TReturnType>(TFieldData requestData = null)
			where TFieldData : FunctionMessage, new();

		UniTask<HexBigInteger> EstimateGas(string methodName, object[] arguments = null, string gas = null,
			string gasPrice = null, string nonce = null);
	}
}