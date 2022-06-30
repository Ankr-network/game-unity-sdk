using AnkrSDK.WebGL.DTO;
using Nethereum.Hex.HexTypes;
using AnkrSDK.Core.Infrastructure;
using Cysharp.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System;
using AnkrSDK.Data;

namespace AnkrSDK.WebGL
{
	public class WebGLWrapper : IDisconnectHandler
	{
		private readonly WebGLCommunicationProtocol _protocol;

		public WebGLWrapper()
		{
			_protocol = new WebGLCommunicationProtocol();
			_protocol.StartReceiveCycle().Forget();
		}
		
		public async UniTask ConnectTo(SupportedWallets wallet, EthereumNetwork chain)
		{
			var id = _protocol.GenerateId();
			var connectionProps = new ConnectionProps
			{
				wallet = wallet.ToString(),
				chain = chain
			};
			
			var payload = JsonConvert.SerializeObject(connectionProps);
			WebGLInterlayer.CreateProvider(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Error)
			{
				throw new Exception(answer.payload);
			}			
		}

		public UniTask Disconnect(bool waitForNewSession = true)
		{
			_protocol.Disconnect();
			return UniTask.CompletedTask;
		}

		public async Task<string> SendTransaction(TransactionData transaction)
		{
			var id = _protocol.GenerateId();
			var payload = JsonConvert.SerializeObject(transaction);
			WebGLInterlayer.SendTransaction(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return answer.payload;
			}

			throw new Exception(answer.payload);
		}

		public async Task<string> GetContractData(TransactionData transaction)
		{
			var id = _protocol.GenerateId();
			var payload = JsonConvert.SerializeObject(transaction);
			WebGLInterlayer.GetContractData(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return answer.payload;
			}

			throw new Exception(answer.payload);
		}

		public async Task<HexBigInteger> EstimateGas(TransactionData transaction)
		{
			var id = _protocol.GenerateId();
			var payload = JsonConvert.SerializeObject(transaction);
			WebGLInterlayer.EstimateGas(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return new HexBigInteger(answer.payload);
			}

			throw new Exception(answer.payload);
		}

		public async Task<string> Sign(DataSignaturePropsDTO signProps)
		{
			var id = _protocol.GenerateId();

			WebGLInterlayer.SignMessage(id, JsonConvert.SerializeObject(signProps));

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return answer.payload;
			}

			throw new Exception(answer.payload);
		}

		public async Task<string> GetDefaultAccount()
		{
			var id = _protocol.GenerateId();
			WebGLInterlayer.GetAddresses(id);

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
			var id = _protocol.GenerateId();
			WebGLInterlayer.GetTransaction(id, transactionHash);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var transactionData = JsonConvert.DeserializeObject<Transaction>(answer.payload);
				return transactionData;
			}

			throw new Exception(answer.payload);
		}
		
		public async Task SwitchChain(EthereumNetwork networkData)
		{
			var id = _protocol.GenerateId();
			var payload = JsonConvert.SerializeObject(networkData);
			WebGLInterlayer.SwitchChain(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Error)
			{
				throw new Exception(answer.payload);
			}
		}
		
		public async Task<FilterLog[]> GetEvents(NewFilterInput filters)
		{
			var id = _protocol.GenerateId();
			var payload = JsonConvert.SerializeObject(filters);
			WebGLInterlayer.GetEvents(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var logs = JsonConvert.DeserializeObject<FilterLog[]>(answer.payload);
				return logs;
			}

			throw new Exception(answer.payload);
		}

		public async Task<TransactionReceipt> GetTransactionReceipt(string transactionHash)
		{
			var id = _protocol.GenerateId();
			WebGLInterlayer.GetTransactionReceipt(id, transactionHash);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return JsonConvert.DeserializeObject<TransactionReceipt>(answer.payload);
			}

			throw new Exception(answer.payload);
		}
	}
}