#if UNITY_WEBGL
using AnkrSDK.WebGL.DTO;
using Cysharp.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace AnkrSDK.WebGL
{
	public class WebGLWrapper
	{
		private readonly WebGLCommunicationProtocol _protocol;

		public WebGLWrapper()
		{
			_protocol = new WebGLCommunicationProtocol();
			_protocol.StartReceiveCycle().Forget();
		}

		public async Task<string> SendTransaction(TransactionData transaction)
		{
			var payload = JsonConvert.SerializeObject(transaction);
			var id = WebGLInterlayer.SendTransaction(payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return answer.payload;
			}

			throw new Exception(answer.payload);
		}

		public async Task<HexBigInteger> EstimateGas(TransactionData transaction)
		{
			var payload = JsonConvert.SerializeObject(transaction);
			var id = WebGLInterlayer.EstimateGas(payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return new HexBigInteger(answer.payload);
			}

			throw new Exception(answer.payload);
		}

		public async Task<string> Sign(DataSignaturePropsDTO signProps)
		{
			var id = WebGLInterlayer.SignMessage(JsonConvert.SerializeObject(signProps));

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return answer.payload;
			}

			throw new Exception(answer.payload);
		}

		public async Task<string> GetDefaultAccount()
		{
			var id = WebGLInterlayer.GetAddresses();

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var addresses = JsonConvert.DeserializeObject<string[]>(answer.payload);
				return addresses.First();
			}

			throw new Exception(answer.payload);
		}

		public async Task<Transaction> GetTransaction(string transactionHash)
		{
			var id = WebGLInterlayer.GetTransaction(transactionHash);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var transactionData = JsonConvert.DeserializeObject<Transaction>(answer.payload);
				return transactionData;
			}

			throw new Exception(answer.payload);
		}

		public async Task<TransactionReceipt> GetTransactionReceipt(string transactionHash)
		{
			var id = WebGLInterlayer.GetTransactionReceipt(transactionHash);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return JsonConvert.DeserializeObject<TransactionReceipt>(answer.payload);
			}

			throw new Exception(answer.payload);
		}
	}
}
#endif