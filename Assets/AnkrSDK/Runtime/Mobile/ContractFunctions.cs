using System.Collections.Generic;
using AnkrSDK.Core.Infrastructure;
using Cysharp.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace AnkrSDK.Mobile
{
	public class ContractFunctions : IContractFunctions
	{
		private readonly IWeb3 _web3Provider;

		public ContractFunctions(IWeb3 web3Provider)
		{
			_web3Provider = web3Provider;
		}

		public UniTask<TReturnType> GetContractData<TFieldData, TReturnType>(string contractAddress,
			TFieldData requestData = null) where TFieldData : FunctionMessage, new()
		{
			var contract = _web3Provider.Eth.GetContractHandler(contractAddress);
			return contract.QueryAsync<TFieldData, TReturnType>(requestData).AsUniTask();
		}
		
		public UniTask<List<EventLog<TEvDto>>> GetEvents<TEvDto>(NewFilterInput filters, string contractAddress)
			where TEvDto : IEventDTO, new()
		{
			var eventHandler = _web3Provider.Eth.GetEvent<TEvDto>(contractAddress);
			return eventHandler.GetAllChangesAsync(filters).AsUniTask();
		}
	}
}