using System.Collections.Generic;
using MirageSDK.Core.Infrastructure;
using MirageSDK.WebGL.Extensions;
using Cysharp.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Contracts.MessageEncodingServices;
using Nethereum.RPC.Eth.DTOs;

namespace MirageSDK.WebGL.Implementation
{
	public class ContractFunctionsWebGL : IContractFunctions
	{
		private readonly WebGLConnect _webGlConnect;

		public ContractFunctionsWebGL(WebGLConnect webGlConnect)
		{
			_webGlConnect = webGlConnect;
		}

		public async UniTask<TReturnType> GetContractData<TFieldData, TReturnType>(string contractAddress,
			TFieldData requestData = null) where TFieldData : FunctionMessage, new()
		{
			var methodEncoder = new FunctionMessageEncodingService<TFieldData>(contractAddress);
			var txData = methodEncoder.CreateCallInput(requestData);
			var response = await _webGlConnect.GetContractData(txData.ToTransactionData());
			return methodEncoder.DecodeSimpleTypeOutput<TReturnType>(response);
		}

		public async UniTask<List<EventLog<TEvDto>>> GetEvents<TEvDto>(NewFilterInput filters, string contractAddress = null)
			where TEvDto : IEventDTO, new()
		{
			var logs = await _webGlConnect.GetEvents(filters);
			var events = logs.DecodeAllEvents<TEvDto>();
			return events;
		}
	}
}
