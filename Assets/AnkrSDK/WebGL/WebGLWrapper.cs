using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using Cysharp.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;

namespace AnkrSDK.WebGL
{
	public class WebGLWrapper
	{
		private static Dictionary<string, UniTaskCompletionSource<WebGLMessageDTO>> completionSources;
		private bool _isCancellationRequested;

		public WebGLWrapper()
		{
			completionSources = new Dictionary<string, UniTaskCompletionSource<WebGLMessageDTO>>();
			Update().Forget();
		}

		public async UniTaskVoid Update()
		{
			while (!_isCancellationRequested)
			{
				ReceiveMessages();

				await UniTask.Yield(PlayerLoopTiming.Update);
			}
		}
    
		public async UniTask<string> SendTransaction(TransactionData transaction)
		{
			var id = GenerateId();
			var payload = JsonConvert.SerializeObject(transaction);
			WebGLInterlayer.SendTransaction(id, payload);
        
			var completionTask = new UniTaskCompletionSource<WebGLMessageDTO>();
			completionSources.Add(id, completionTask);

			var answer = await completionTask.Task;
			completionSources.Remove(id);

			return answer.payload;
		}

		public async UniTask<string> Sign(string message)
		{
			var id = GenerateId();
			WebGLInterlayer.SignMessage(id, message);
        
			var completionTask = new UniTaskCompletionSource<WebGLMessageDTO>();
			completionSources.Add(id, completionTask);

			var answer = await completionTask.Task;
			completionSources.Remove(id);

			return answer.payload;
		}
		
		public async UniTask<string> GetDefaultAccount()
		{
			var id = GenerateId();
			WebGLInterlayer.GetAddresses(id);
        
			var completionTask = new UniTaskCompletionSource<WebGLMessageDTO>();
			completionSources.Add(id, completionTask);

			var answer = await completionTask.Task;
			completionSources.Remove(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var addresses = JsonConvert.DeserializeObject<string[]>(answer.payload);
				return addresses.First();
			}

			throw new Exception(answer.payload);
		}

		private string GenerateId()
		{
			return Guid.NewGuid().ToString();
		}

		private void ReceiveMessages()
		{
			var json = WebGLInterlayer.GetResponses();
			var messages = JsonConvert.DeserializeObject<WebGLMessageDTO[]>(json);
			if (messages.Length > 0)
			{
				foreach (var message in messages)
				{
					if (completionSources.ContainsKey(message.id))
					{
						var completionSource = completionSources[message.id];
						completionSource.TrySetResult(message);
					}
				}
			}
		}
	}
}