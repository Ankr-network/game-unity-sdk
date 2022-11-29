using System.Collections.Generic;
using System.Threading.Tasks;
using MirageSDK.Core.Infrastructure;
using MirageSDK.WebGL.Extensions;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Contracts.MessageEncodingServices;
using Nethereum.RPC.Eth.DTOs;

namespace MirageSDK.WebGL.Implementation
{
	public class ContractFunctionsWebGL : IContractFunctions
	{
		private readonly WebGLWrapper _webGlWrapper;

		public ContractFunctionsWebGL(WebGLWrapper webGlWrapper)
		{
			_webGlWrapper = webGlWrapper;
		}

		public async Task<TReturnType> GetContractData<TFieldData, TReturnType>(string contractAddress,
			TFieldData requestData = null) where TFieldData : FunctionMessage, new()
		{
			var methodEncoder = new FunctionMessageEncodingService<TFieldData>(contractAddress);
			var txData = methodEncoder.CreateCallInput(requestData);
			var response = await _webGlWrapper.GetContractData(txData.ToTransactionData());
			return methodEncoder.DecodeSimpleTypeOutput<TReturnType>(response);
		}
		
		public async Task<List<EventLog<TEvDto>>> GetEvents<TEvDto>(NewFilterInput filters, string contractAddress = null)
			where TEvDto : IEventDTO, new()
		{
			var logs = await _webGlWrapper.GetEvents(filters);
			var events = logs.DecodeAllEvents<TEvDto>();
			return events;
		}
	}
}
