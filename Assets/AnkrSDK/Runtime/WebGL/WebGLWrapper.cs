using AnkrSDK.WebGL.DTO;
using Nethereum.Hex.HexTypes;
using AnkrSDK.Core.Infrastructure;
using Cysharp.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;
using System.Linq;
using System;
using System.Numerics;
using AnkrSDK.Data;
using Newtonsoft.Json;

namespace AnkrSDK.WebGL
{
	public class WebGLWrapper : IWalletHandler
	{
		private readonly WebGLCommunicationProtocol _protocol;

		public WebGLWrapper()
		{
			_protocol = new WebGLCommunicationProtocol();
			_protocol.StartReceiveCycle().Forget();
		}

		public async UniTask ConnectTo(Wallet wallet, EthereumNetwork chain)
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
		
		public async UniTask<WalletsStatus> GetWalletsStatus()
		{
			var id = _protocol.GenerateId();
			WebGLInterlayer.GetWalletsStatus(id);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var payload = JsonConvert.DeserializeObject<WalletsStatus>(answer.payload);
				return payload;
			}

			throw new Exception(answer.payload);
		}

		public UniTask Disconnect(bool waitForNewSession = true)
		{
			_protocol.Disconnect();
			return UniTask.CompletedTask;
		}

		public async UniTask<string> SendTransaction(TransactionData transaction)
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

		public async UniTask<string> GetContractData(TransactionData transaction)
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

		public async UniTask<HexBigInteger> EstimateGas(TransactionData transaction)
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

		public async UniTask<string> Sign(DataSignaturePropsDTO signProps)
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

		public async UniTask<string> GetDefaultAccount()
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

		public async UniTask<BigInteger> GetChainId()
		{
			var id = _protocol.GenerateId();
			WebGLInterlayer.GetChainId(id);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var payload = JsonConvert.DeserializeObject<WebGLCallAnswer<HexBigInteger>>(answer.payload);
				return payload.Result.Value;
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<Transaction> GetTransaction(string transactionHash)
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

		public async UniTask SwitchChain(EthereumNetwork networkData)
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

		public async UniTask<TReturnType> CallMethod<TReturnType>(WebGLCallObject callObject)
		{
			var id = _protocol.GenerateId();
			var payload = JsonConvert.SerializeObject(callObject);
			WebGLInterlayer.CallMethod(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var callAnswer = JsonConvert.DeserializeObject<WebGLCallAnswer<TReturnType>>(answer.payload);

				return callAnswer.Result;
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<FilterLog[]> GetEvents(NewFilterInput filters)
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

		public async UniTask<TransactionReceipt> GetTransactionReceipt(string transactionHash)
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