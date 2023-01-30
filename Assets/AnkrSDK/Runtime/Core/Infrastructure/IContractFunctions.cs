using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IContractFunctions
	{
		UniTask<TReturnType> GetContractData<TFieldData, TReturnType>(string contractAddress,
			TFieldData requestData = null) where TFieldData : FunctionMessage, new();

		UniTask<List<EventLog<TEvDto>>> GetEvents<TEvDto>(NewFilterInput filters, string contractAddress = null)
			where TEvDto : IEventDTO, new();
	}
}