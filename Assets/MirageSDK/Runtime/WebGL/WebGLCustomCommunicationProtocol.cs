using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using MirageSDK.WebGL.DTO;
using Cysharp.Threading.Tasks;
using MirageSDK.Data;
using MirageSDK.WalletConnect.VersionShared.Models.Ethereum;
using MirageSDK.WebGL.Infrastructure;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;

namespace MirageSDK.WebGL
{
	internal class WebGLCustomCommunicationProtocol : IWebGLCommunicationProtocol
	{
		private const string GetBalanceMethodName = "eth.getBalance";
		private const string GetBlockMethodName = "eth.getBlock";
		private const string GetBlockNumberMethodName = "eth.getBlockNumber";
		private const string GetBlockTransactionCountMethodName = "eth.getBlockTransactionCount";

		private readonly Dictionary<string, UniTaskCompletionSource<WebGLMessageDTO>> _completionSources =
			new Dictionary<string, UniTaskCompletionSource<WebGLMessageDTO>>();

		private CancellationTokenSource _cancellationTokenSource;

		public void Init()
		{
			StartReceiveCycle().Forget();
		}

		public async UniTask ConnectTo(Wallet wallet, EthereumNetwork chain)
		{
			var id = GenerateId();
			var connectionProps = new ConnectionProps
			{
				wallet = wallet.ToString(),
				chain = chain
			};

			var payload = JsonConvert.SerializeObject(connectionProps);
			WebGLInterlayer.CreateProvider(id, payload);

			var answer = await WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Error)
			{
				throw new Exception(answer.payload);
			}
		}

		public void Disconnect()
		{
			_cancellationTokenSource.Cancel();
			if (_completionSources.Count > 0)
			{
				CompleteAllSources();
			}
		}

		public async UniTask<WalletsStatus> GetWalletsStatus()
		{
			var id = GenerateId();
			WebGLInterlayer.GetWalletsStatus(id);

			var answer = await WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var payload = JsonConvert.DeserializeObject<WalletsStatus>(answer.payload);
				return payload;
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<string> SendTransaction(TransactionData transaction)
		{
			var id = GenerateId();
			var payload = JsonConvert.SerializeObject(transaction);
			WebGLInterlayer.SendTransaction(id, payload);

			var answer = await WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return answer.payload;
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<string> GetContractData(TransactionData transaction)
		{
			var id = GenerateId();
			var payload = JsonConvert.SerializeObject(transaction);
			WebGLInterlayer.GetContractData(id, payload);

			var answer = await WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return answer.payload;
			}

			throw new Exception(answer.payload);
		}

		private string GenerateId()
		{
			var id = Guid.NewGuid().ToString();

			var completionTask = new UniTaskCompletionSource<WebGLMessageDTO>();
			_completionSources.Add(id, completionTask);

			return id;
		}

		private async UniTask<WebGLMessageDTO> WaitForAnswer(string id)
		{
			var completionTask = _completionSources[id];
			var answer = await completionTask.Task;
			_completionSources.Remove(id);

			return answer;
		}

		private async UniTaskVoid StartReceiveCycle()
		{
			_cancellationTokenSource = new CancellationTokenSource();
			var token = _cancellationTokenSource.Token;
			while (!token.IsCancellationRequested)
			{
				ReceiveMessages();

				await UniTask.Yield(PlayerLoopTiming.Update);
			}
		}

		public async UniTask<HexBigInteger> EstimateGas(TransactionData transaction)
		{
			var id = GenerateId();
			var payload = JsonConvert.SerializeObject(transaction);
			WebGLInterlayer.EstimateGas(id, payload);

			var answer = await WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return new HexBigInteger(answer.payload);
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<string> Sign(DataSignaturePropsDTO signProps)
		{
			var id = GenerateId();

			WebGLInterlayer.SignMessage(id, JsonConvert.SerializeObject(signProps));

			var answer = await WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return answer.payload;
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<string> GetDefaultAccount(string network = null)
		{
			var id = GenerateId();
			WebGLInterlayer.GetAddresses(id);

			var answer = await WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var addresses = JsonConvert.DeserializeObject<string[]>(answer.payload);
				return addresses.First();
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<string> GetDefaultAccount()
		{
			var id = GenerateId();
			WebGLInterlayer.GetAddresses(id);

			var answer = await WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var addresses = JsonConvert.DeserializeObject<string[]>(answer.payload);
				return addresses.First();
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<BigInteger> GetChainId()
		{
			var id = GenerateId();
			WebGLInterlayer.RequestChainId(id);

			var answer = await WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var payload = JsonConvert.DeserializeObject<WebGLCallAnswer<HexBigInteger>>(answer.payload);
				return payload.Result.Value;
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<Transaction> GetTransaction(string transactionHash)
		{
			var id = GenerateId();
			WebGLInterlayer.GetTransaction(id, transactionHash);

			var answer = await WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var transactionData = JsonConvert.DeserializeObject<Transaction>(answer.payload);
				return transactionData;
			}

			throw new Exception(answer.payload);
		}

		public async UniTask AddChain(EthChainData networkData)
		{
			var id = GenerateId();
			var payload = JsonConvert.SerializeObject(networkData);
			WebGLInterlayer.AddChain(id, payload);

			var answer = await WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Error)
			{
				throw new Exception(answer.payload);
			}
		}

		public async UniTask UpdateChain(EthUpdateChainData networkData)
		{
			var id = GenerateId();
			var payload = JsonConvert.SerializeObject(networkData);
			WebGLInterlayer.AddChain(id, payload);

			var answer = await WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Error)
			{
				throw new Exception(answer.payload);
			}
		}

		public async UniTask SwitchChain(EthChain networkData)
		{
			var id = GenerateId();
			var payload = JsonConvert.SerializeObject(networkData);
			WebGLInterlayer.SwitchChain(id, payload);

			var answer = await WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Error)
			{
				throw new Exception(answer.payload);
			}
		}

		public async UniTask<BigInteger> GetBalance()
		{
			var address = await GetDefaultAccount();
			var callObject = new WebGLCallObject
			{
				Path = GetBalanceMethodName, Args = address != null
					? new[]
					{
						address
					}
					: null
			};

			return await CallMethod<BigInteger>(callObject);
		}

		public UniTask<BigInteger> GetBlockNumber()
		{
			var callObject = new WebGLCallObject
			{
				Path = GetBlockNumberMethodName
			};

			return CallMethod<BigInteger>(callObject);
		}

		public UniTask<BigInteger> GetBlockTransactionCount(string blockId)
		{
			var callObject = new WebGLCallObject
			{
				Path = GetBlockTransactionCountMethodName, Args = new[]
				{
					blockId
				}
			};

			return CallMethod<BigInteger>(callObject);
		}

		public UniTask<TResultType> GetBlock<TResultType>(string blockId, bool returnTransactionObjects)
		{
			var callObject = new WebGLCallObject
			{
				Path = GetBlockMethodName, Args = new object[]
				{
					blockId, returnTransactionObjects
				}
			};
			return CallMethod<TResultType>(callObject);
		}

		private async UniTask<TReturnType> CallMethod<TReturnType>(WebGLCallObject callObject)
		{
			var id = GenerateId();
			var payload = JsonConvert.SerializeObject(callObject);
			WebGLInterlayer.CallMethod(id, payload);

			var answer = await WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var callAnswer = JsonConvert.DeserializeObject<WebGLCallAnswer<TReturnType>>(answer.payload);

				return callAnswer.Result;
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<FilterLog[]> GetEvents(NewFilterInput filters)
		{
			var id = GenerateId();
			var payload = JsonConvert.SerializeObject(filters);
			WebGLInterlayer.GetEvents(id, payload);

			var answer = await WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var logs = JsonConvert.DeserializeObject<FilterLog[]>(answer.payload);
				return logs;
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<TransactionReceipt> GetTransactionReceipt(string transactionHash)
		{
			var id = GenerateId();
			WebGLInterlayer.GetTransactionReceipt(id, transactionHash);

			var answer = await WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return JsonConvert.DeserializeObject<TransactionReceipt>(answer.payload);
			}

			throw new Exception(answer.payload);
		}

		private void ReceiveMessages()
		{
			var json = WebGLInterlayer.GetResponses();
			if (String.IsNullOrEmpty(json))
			{
				return;
			}

			var messages = JsonConvert.DeserializeObject<WebGLMessageDTO[]>(json);

			foreach (var message in messages)
			{
				if (_completionSources.ContainsKey(message.id))
				{
					var completionSource = _completionSources[message.id];
					completionSource.TrySetResult(message);
				}
			}
		}

		private void CompleteAllSources()
		{
			foreach (var entry in _completionSources)
			{
				var answer = new WebGLMessageDTO
				{
					id = entry.Key,
					status = WebGLMessageStatus.Error,
					payload = "Answer was not received before protocol disconnected"
				};
				entry.Value.TrySetResult(answer);
			}

			_completionSources.Clear();
		}
	}
}