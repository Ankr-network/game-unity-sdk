using System.Threading.Tasks;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.WebGL;
using AnkrSDK.WebGL.Extensions;
using Nethereum.Contracts;
using Nethereum.Contracts.MessageEncodingServices;
using Newtonsoft.Json;
using UnityEngine;

namespace AnkrSDK.Core.Implementation
{
	public class ContractFunctionsWebGL : IContractFunctions
	{
		private readonly WebGLWrapper _webGlWrapper;
		
		public ContractFunctionsWebGL(WebGLWrapper webGlWrapper)
		{
			_webGlWrapper = webGlWrapper;
		}

		public async Task<TReturnType> GetContractData<TFieldData, TReturnType>(string contractAddress, TFieldData requestData = null) where TFieldData : FunctionMessage, new()
		{
			var methodEncoder = new FunctionMessageEncodingService<TFieldData>(contractAddress);
			var txData = methodEncoder.CreateCallInput(requestData);
			var response = await _webGlWrapper.GetContractData(txData.ToTransactionData());
			return methodEncoder.DecodeSimpleTypeOutput<TReturnType>(response);
		}
	}
}